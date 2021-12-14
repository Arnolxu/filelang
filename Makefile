build:
	@dotnet build

install:
	@sudo mkdir -p /etc/filelang
	@sudo cp bin/Debug/net6.0/{filelang,filelang.runtimeconfig.json,filelang.deps.json,filelang.dll} /etc/filelang/
	@sudo cp fl.sh /usr/bin/fl
	@sudo chmod +x /usr/bin/fl
	@sudo mkdir -p /usr/lib/fl

uninstall:
	@sudo rm -rf /etc/filelang /usr/bin/fl /usr/lib/fl
