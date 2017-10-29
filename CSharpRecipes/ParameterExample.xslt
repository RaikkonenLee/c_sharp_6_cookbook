<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" indent="yes" />
  <xsl:param name="storeTitle"/>
  <xsl:param name="pageDate"/>

  <xsl:template match="ParameterExample">
    <html>
      <head/>
      <body>
        <h3>
          <xsl:text>Brought to you by </xsl:text>
          <xsl:value-of select="$storeTitle"/>
          <xsl:text> on </xsl:text>
          <xsl:value-of select="$pageDate"/>
          <xsl:text> &#xd;&#xa;</xsl:text>
        </h3>
        <br/>
        <table border="2">
          <thead>
            <tr>
              <td>
                <b>Heroes</b>
              </td>
              <td>
                <b>Edition</b>
              </td>
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
        <xsl:value-of select="@name"/>
      </td>
      <td>
        <xsl:value-of select="@edition"/>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
