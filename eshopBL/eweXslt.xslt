<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="specifications">
              <table class="grid">
                
                <xsl:for-each select="attribute_group">
                  <tr class="attributeGroup">
                    <td colspan="2">
                      <xsl:value-of select="@name"/>
                    </td>
                  </tr>
                  <xsl:for-each select="current()/attribute">
                    <tr>
                      <td class="attributeName">
                        <xsl:value-of select="@name"/>
                      </td>
                      <td>
                        <xsl:value-of select="value"/>
                      </td>
                    </tr>
                  </xsl:for-each>
                </xsl:for-each>
              </table>
        </xsl:template>
      </xsl:stylesheet>
