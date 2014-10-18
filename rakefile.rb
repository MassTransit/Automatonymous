COPYRIGHT = "Copyright 2011-2013 Chris Patterson, All rights reserved."

include FileTest
require 'albacore'
require 'semver'

internal_files = Dir[File.join(File.expand_path("src"), 'Automatonymous/Internals/**/*.cs')]
if(!internal_files.any?)
  #you didn't git submodule update --init - here let me help you
  sh 'git submodule update --init' unless internal_files.any?
end

PRODUCT = 'Automatonymous'
CLR_TOOLS_VERSION = 'v4.0.30319'
OUTPUT_PATH = 'bin/Release'
NETCORE45_FRAMEWORK = '.NET Framework, Version=v4.5'

props = {
  :src => File.expand_path("src"),
  :output => File.expand_path("build_output"),
  :artifacts => File.expand_path("build_artifacts"),
  :projects => ["Automatonymous"],
  :lib => File.expand_path("lib"),
  :keyfile => File.expand_path("Automatonymous.snk")
}

desc "Cleans, compiles, il-merges, unit tests, prepares examples, packages zip"
task :all => [:default, :package]

desc "**Default**, compiles and runs tests"
task :default => [:clean, :nuget_restore, :compile, :tests, :compile_net45fx, :nuget]

desc "Update the common version information for the build. You can call this task without building."
assemblyinfo :global_version do |asm|
  # Assembly file config
  asm.product_name = PRODUCT
  asm.description = "Automatonymous is an open source state machine library."
  asm.version = FORMAL_VERSION
  asm.file_version = FORMAL_VERSION
  asm.custom_attributes :AssemblyInformationalVersion => "#{BUILD_VERSION}",
  :ComVisibleAttribute => false,
  :CLSCompliantAttribute => true
  asm.copyright = COPYRIGHT
  asm.output_file = 'src/SolutionVersion.cs'
  asm.namespaces "System", "System.Reflection", "System.Runtime.InteropServices"
end

#desc "Update the common version information for the build. You can call this task without building."
#asmver :global_version do |asm|
#  # Assembly file config
#  asm.file_path = 'src/SolutionVersion.cs'
#  
#  #asm.namespaces "System", "System.Reflection", "System.Runtime.InteropServices"
#  
#  asm.attributes assembly_description: "Automatonymous is an open source state machine library.",
#                 assembly_version: FORMAL_VERSION,
#                 assembly_file_version: FORMAL_VERSION,
#                 assembly_informational_version: "#{BUILD_VERSION}",
#                 assembly_copyright: COPYRIGHT,
#                 assembly_product: PRODUCT,
#                 assembly_title: PRODUCT,
#                 com_visible: false,
#                 CLS_compliant: true
#
#  #asm.out = StringIO.new
#end

desc "Prepares the working directory for a new build"
task :clean do
	FileUtils.rm_rf props[:output]
	waitfor { !exists?(props[:output]) }

	FileUtils.rm_rf props[:artifacts]
	waitfor { !exists?(props[:artifacts]) }

	Dir.mkdir props[:output]
	Dir.mkdir props[:artifacts]
end

desc "Cleans, versions, compiles the application and generates build_output/."
task :compile => [:versioning, :global_version, :build] do
  copyOutputFiles File.join(props[:src], "Automatonymous/bin/Release"), "Automatonymous.{dll,pdb,xml}", File.join(props[:output], 'net-4.0')
  copyOutputFiles File.join(props[:src], "MassTransit/Automatonymous.MassTransitIntegration/bin/Release"), "Automatonymous.MassTransitIntegration.{dll,pdb,xml}", File.join(props[:output], 'net-4.0')
  copyOutputFiles File.join(props[:src], "Automatonymous.NHibernateIntegration/bin/Release"), "Automatonymous.NHibernateIntegration.{dll,pdb,xml}", File.join(props[:output], 'net-4.0')
  copyOutputFiles File.join(props[:src], "Automatonymous.Visualizer/bin/Release"), "Automatonymous.Visualizer.{dll,pdb,xml}", File.join(props[:output], 'net-4.0')
end

desc "Cleans, versions, compiles the application and generates build_output/."
task :compile_net45fx => [:global_version, :build_net45fx] do
	copyOutputFiles File.join(props[:src], "Automatonymous/bin/Release/win8"), "Automatonymous.{dll,pdb,xml}", File.join(props[:output], 'win8')
end

desc "Only compiles the application."
msbuild :build do |msb|
  msb.properties :Configuration => "Release",
    :Platform => 'Any CPU'
  msb.use :net4
  msb.targets :Clean, :Build
  msb.properties[:SignAssembly] = 'true'
  msb.properties[:AssemblyOriginatorKeyFile] = props[:keyfile]
  msb.solution = 'src/Automatonymous.sln'
end

desc "Only compiles the application for .NET 4.5 FX CORE."
msbuild :build_net45fx do |msb|
  msb.properties :Configuration => "Release45",
    :SignAssembly => 'true',
    :AssemblyOriginatorKeyFile => props[:keyfile]
  msb.use :net4
  msb.targets :Clean, :Build
  msb.solution = 'src/Automatonymous/Automatonymous.csproj'
end

def copyOutputFiles(fromDir, filePattern, outDir)
	FileUtils.mkdir_p outDir unless exists?(outDir)
	Dir.glob(File.join(fromDir, filePattern)){|file|
		copy(file, outDir) if File.file?(file)
	}
end

desc "Runs unit tests"
nunit :tests => [:compile] do |nunit|
          nunit.command = File.join('src', 'packages','NUnit.Runners.2.6.3', 'tools', 'nunit-console.exe')
          nunit.options = "/framework=#{CLR_TOOLS_VERSION}", '/nothread', '/nologo', '/labels', "\"/xml=#{File.join(props[:artifacts], 'nunit-test-results.xml')}\""
          nunit.assemblies = FileList["tests/Automatonymous.Tests.dll", File.join(props[:src], "MassTransit/MassTransit.AutomatonymousTests/bin/Release", "MassTransit.AutomatonymousTests.dll"), File.join(props[:src], "NHibernate.AutomatonymousTests/bin/Release", "NHibernate.AutomatonymousTests.dll")]
end

task :package => [:nuget]

desc "ZIPs up the build results."
zip :zip_output do |zip|
	zip.directories_to_zip = [props[:stage]]
	zip.output_file = "Automatonymous-#{NUGET_VERSION}.zip"
	zip.output_path = [props[:artifacts]]
end

desc "restores missing packages"
msbuild :nuget_restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = File.join(props[:src], "Automatonymous.Tests", "Automatonymous.Tests.csproj")
end

desc "restores missing packages"
msbuild :nuget_restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = File.join(props[:src], "MassTransit", "MassTransit.AutomatonymousTests", "MassTransit.AutomatonymousTests.csproj")
end

desc "restores missing packages"
msbuild :nuget_restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = File.join(props[:src], "MassTransit", "Automatonymous.MassTransitIntegration", "Automatonymous.MassTransitIntegration.csproj")
end

desc "restores missing packages"
msbuild :nuget_restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = File.join(props[:src], "Automatonymous.NHibernateIntegration", "Automatonymous.NHibernateIntegration.csproj")
end

desc "restores missing packages"
msbuild :nuget_restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = File.join(props[:src], "NHibernate.AutomatonymousTests", "NHibernate.AutomatonymousTests.csproj")
end

desc "restores missing packages"
msbuild :nuget_restore do |msb|
  msb.use :net4
  msb.targets :RestorePackages
  msb.solution = File.join(props[:src], "Automatonymous.Visualizer", "Automatonymous.Visualizer.csproj")
end

desc "Builds the nuget package"
task :nuget => ['create_nuspec', 'create_nuspec_masstransit', 'create_nuspec_nhibernate', 'create_nuspec_quickgraph'] do
	sh "#{File.join(props[:src],'.nuget','nuget.exe')} pack #{props[:artifacts]}/Automatonymous.nuspec /Symbols /OutputDirectory #{props[:artifacts]}"
  sh "#{File.join(props[:src],'.nuget','nuget.exe')} pack #{props[:artifacts]}/Automatonymous.MassTransit.nuspec /Symbols /OutputDirectory #{props[:artifacts]}"
  sh "#{File.join(props[:src],'.nuget','nuget.exe')} pack #{props[:artifacts]}/Automatonymous.NHibernate.nuspec /Symbols /OutputDirectory #{props[:artifacts]}"
	sh "#{File.join(props[:src],'.nuget','nuget.exe')} pack #{props[:artifacts]}/Automatonymous.Visualizer.nuspec /Symbols /OutputDirectory #{props[:artifacts]}"
end

nuspec :create_nuspec do |nuspec|
  nuspec.id = 'Automatonymous'
  nuspec.version = NUGET_VERSION
  nuspec.authors = 'Chris Patterson'
  nuspec.description = 'Automatonymous is a state machine library for .NET'
  nuspec.title = 'Automatonymous'
  nuspec.projectUrl = 'http://github.com/MassTransit/Automatonymous'
  nuspec.language = "en-US"
  nuspec.licenseUrl = "http://www.apache.org/licenses/LICENSE-'Chris Patterson, Dru Sellers, Travis Smith'0"
  nuspec.requireLicenseAcceptance = "false"
  nuspec.dependency "Taskell", "0.1.2"
  nuspec.output_file = File.join(props[:artifacts], 'Automatonymous.nuspec')
  add_files props[:output], 'Automatonymous.{dll,pdb,xml}', nuspec
  nuspec.file(File.join(props[:src], "Automatonymous\\**\\*.cs").gsub("/","\\"), "src")
end

nuspec :create_nuspec_masstransit do |nuspec|
  nuspec.id = 'Automatonymous.MassTransit'
  nuspec.version = NUGET_VERSION
  nuspec.authors = 'Chris Patterson'
  nuspec.description = 'Integration assembly to support Automatonymous sagas, a state machine library for .NET'
  nuspec.title = 'Automatonymous.MassTransit'
  nuspec.projectUrl = 'http://github.com/MassTransit/Automatonymous'
  nuspec.language = "en-US"
  nuspec.licenseUrl = "http://www.apache.org/licenses/LICENSE-2.0"
  nuspec.requireLicenseAcceptance = "false"
  nuspec.dependency "MassTransit", "2.9.8"
  nuspec.dependency "Taskell", "0.1.2"
  nuspec.dependency "Automatonymous", NUGET_VERSION
  nuspec.output_file = File.join(props[:artifacts], 'Automatonymous.MassTransit.nuspec')
  add_files props[:output], 'Automatonymous.MassTransitIntegration.{dll,pdb,xml}', nuspec
  nuspec.file(File.join(props[:src], "MassTransit\\Automatonymous.MassTransitIntegration\\**\\*.cs").gsub("/","\\"), "src")
end

nuspec :create_nuspec_nhibernate do |nuspec|
  nuspec.id = 'Automatonymous.NHibernate'
  nuspec.version = NUGET_VERSION
  nuspec.authors = 'Chris Patterson'
  nuspec.description = 'Integration assembly to support Automatonymous NHibernate, a state machine library for .NET'
  nuspec.title = 'Automatonymous.NHibernate'
  nuspec.projectUrl = 'http://github.com/MassTransit/Automatonymous'
  nuspec.language = "en-US"
  nuspec.licenseUrl = "http://www.apache.org/licenses/LICENSE-2.0"
  nuspec.requireLicenseAcceptance = "false"
  nuspec.dependency "NHibernate", "4.0.1.4000"
  nuspec.dependency "Iesi.Collections", "4.0.1.4000"
  nuspec.dependency "Automatonymous", NUGET_VERSION
  nuspec.output_file = File.join(props[:artifacts], 'Automatonymous.NHibernate.nuspec')
  add_files props[:output], 'Automatonymous.NHibernateIntegration.{dll,pdb,xml}', nuspec
  nuspec.file(File.join(props[:src], "Automatonymous.NHibernateIntegration\\**\\*.cs").gsub("/","\\"), "src")
end

nuspec :create_nuspec_quickgraph do |nuspec|
  nuspec.id = 'Automatonymous.Visualizer'
  nuspec.version = NUGET_VERSION
  nuspec.authors = 'Chris Patterson'
  nuspec.description = 'Visualization assembly to support Automatonymous, a state machine library for .NET'
  nuspec.title = 'Automatonymous.Visualizer'
  nuspec.projectUrl = 'http://github.com/MassTransit/Automatonymous'
  nuspec.language = "en-US"
  nuspec.licenseUrl = "http://www.apache.org/licenses/LICENSE-2.0"
  nuspec.requireLicenseAcceptance = "false"
  nuspec.dependency "QuickGraph", "3.6.61119"
  nuspec.dependency "Automatonymous", NUGET_VERSION
  nuspec.output_file = File.join(props[:artifacts], 'Automatonymous.Visualizer.nuspec')
  add_files props[:output], 'Automatonymous.Visualizer.{dll,pdb,xml}', nuspec
  nuspec.file(File.join(props[:src], "Automatonymous.Visualizer\\**\\*.cs").gsub("/","\\"), "src")
end

def project_outputs(props)
	props[:projects].map{ |p| "src/#{p}/bin/#{BUILD_CONFIG}/#{p}.dll" }.
		concat( props[:projects].map{ |p| "src/#{p}/bin/#{BUILD_CONFIG}/#{p}.exe" } ).
		find_all{ |path| exists?(path) }
end


def add_files stage, what_dlls, nuspec
  [['net40', 'net-4.0'], ['.NETCore45', 'win8']].each{|fw|
    takeFrom = File.join(stage, fw[1], what_dlls)
    Dir.glob(takeFrom).each do |f|
      nuspec.file(f.gsub("/", "\\"), "lib\\#{fw[0]}")
    end
  }
end

def commit_data
  begin
    commit = `git rev-parse --short HEAD`.chomp()[0,6]
    git_date = `git log -1 --date=iso --pretty=format:%ad`
    commit_date = DateTime.parse( git_date ).strftime("%Y-%m-%d %H%M%S")
  rescue Exception => e
    puts e.inspect
    commit = (ENV['BUILD_VCS_NUMBER'] || "000000")[0,6]
    commit_date = Time.new.strftime("%Y-%m-%d %H%M%S")
  end
  [commit, commit_date]
end

task :versioning do
  ver = SemVer.find
  revision = (ENV['BUILD_NUMBER'] || ver.patch).to_i
  var = SemVer.new(ver.major, ver.minor, revision, ver.special)
  
  # extensible number w/ git hash
  ENV['BUILD_VERSION'] = BUILD_VERSION = ver.format("%M.%m.%p%s") + ".#{commit_data()[0]}"
  
  # nuget (not full semver 2.0.0-rc.1 support) see http://nuget.codeplex.com/workitem/1796
  ENV['NUGET_VERSION'] = NUGET_VERSION = ver.format("%M.%m.%p%s")
  
  # purely M.m.p format
  ENV['FORMAL_VERSION'] = FORMAL_VERSION = "#{ SemVer.new(ver.major, ver.minor, revision).format "%M.%m.%p"}"
  puts "##teamcity[buildNumber '#{BUILD_VERSION}']" # tell teamcity our decision
end

def add_files stage, what_dlls, nuspec
  [['net40', 'net-4.0'], ['.NETCore45', 'win8']].each{|fw|
    takeFrom = File.join(stage, fw[1], what_dlls)
    Dir.glob(takeFrom).each do |f|
      nuspec.file(f.gsub("/", "\\"), "lib\\#{fw[0]}")
    end
  }
end

def waitfor(&block)
	checks = 0

	until block.call || checks >10
		sleep 0.5
		checks += 1
	end

	raise 'Waitfor timeout expired. Make sure that you aren\'t running something from the build output folders, or that you have browsed to it through Explorer.' if checks > 10
end
