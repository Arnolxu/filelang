build:
	@dotnet build

run:
	@./bin/Debug/net6.0/filelang examples/example1

install:
	@sudo mkdir -p /etc/filelang
	@sudo cp bin/Debug/net6.0/{filelang,filelang.runtimeconfig.json,filelang.deps.json,filelang.dll} /etc/filelang/
	@sudo cp fl.sh /usr/bin/fl
	@sudo chmod +x /usr/bin/fl

uninstall:
	@sudo rm -rf /etc/filelang /usr/bin/fl
