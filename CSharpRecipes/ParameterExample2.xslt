<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="text"/>
	<xsl:param name="storeTitle"/>
	<xsl:param name="pageDate"/>

	<xsl:template match="ParameterExample">
	        <html>	
	            <head />
	            <body title="Comic Books">
		<h1>Brought to you by <xsl:value-of select="$storeTitle"/> on <xsl:value-of select="$pageDate"/></h1>
	                   <table border="1">
                                    <thead>
                                        <tr>
                                            <td>Heroes</td>
                                            <td>Edition</td>
                                        </tr>
                                    </thead>
                                    <tbody>
		  <xsl:apply-templates/>
                                    </tbody>
                                </table>
	            </body>
	        </html>
	</xsl:template>
	
	<xsl:template match="ComicBook">
                                            <tr>
                                                <td>
                                                       <xsl:value-of select="@name" />
                                                </td>
                                                <td>
                                                    <xsl:value-of select="@edition"/>
                                                </td>
                                            </tr>
	</xsl:template>	
</xsl:stylesheet>
  

