<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xslext="urn:xslext">
	<xsl:template match="/">
		<xsl:element name="PublishedWorks">
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="Book">
		<Book>
			<xsl:attribute name ="name">
				<xsl:value-of select="@name"/>
			</xsl:attribute>
			<xsl:for-each select="Chapter">
				<Chapter>
					<xsl:value-of select="xslext:GetErrata(.)"/>
				</Chapter>
			</xsl:for-each>
		</Book>
	</xsl:template>
</xsl:stylesheet>

