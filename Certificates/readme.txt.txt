In order to use the SSL version of the TCP Server and Client, you need to install the self signed certificate and PFX file into your certificate store.

The certificate needs to be in both the Personal and Trusted Root certificate store.  If you search on "Importing Certificates Using MMC" on MSDN you will see plenty of guidance on how to do this.


These were the commands to create the certificate and PFX files (run from Admin Developer Command Prompt):

	makecert.exe -n "CN=CSharpCookBook.net" -r -pe -a sha512 -len 4096 -cy authority -sv CSCBNet.pvk CSCBNet.cer
 
	pvk2pfx.exe -pvk CSCBNet.pvk -spc CSCBNet.cer -pfx CSCBNet.pfx -po CSCB