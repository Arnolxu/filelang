build:
	@dotnet build

install:
	@mkdir -p /etc/filelang
	@cp bin/Debug/net6.0/{filelang,filelang.runtimeconfig.json,filelang.deps.json,filelang.dll} /etc/filelang/
	@cp fl.sh /usr/bin/fl
	@chmod +x /usr/bin/fl
	@mkdir -p /usr/lib/fl

uninstall:
	@rm -rf /etc/filelang /usr/bin/fl /usr/lib/fl
