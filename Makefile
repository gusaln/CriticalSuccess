CONSOLETARGET=$(HOME)/.local/bin/criticalSuccess

.PHONY: all console clean

all: console

console:
	dotnet publish -c Release -r linux-x64 --no-self-contained -o build src/CriticalSuccess.Console
	mv build/CriticalSuccess.Console $(CONSOLETARGET)

clean:
	dotnet clean src