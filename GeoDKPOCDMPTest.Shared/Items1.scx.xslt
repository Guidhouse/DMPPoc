<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="templates">
    namespace Sitecore.Templates
    {
    <xsl:for-each select="template">
      <xsl:call-template name="renderTemplate" />
    </xsl:for-each>
    }
  </xsl:template>

  <xsl:template name="renderTemplate">
    public partial class <xsl:value-of select="@safename"/>
    {
    <xsl:for-each select="field">
      <xsl:call-template name="renderFieldIds" />
    </xsl:for-each>

    public <xsl:value-of select="@safename"/>(Sitecore.Data.Items.Item item)
    {
    this.InnerItem = item;
    }

    public Sitecore.Data.Items.Item InnerItem { get; set; }

    <xsl:for-each select="field">
      <xsl:call-template name="renderField" />
    </xsl:for-each>
    }
  </xsl:template>

  <xsl:template name="renderField">
    public string <xsl:value-of select="@safename"/>
    {
    get { return this.InnerItem[<xsl:value-of select="@safename"/>Id]; }
    set { this.InnerItem[<xsl:value-of select="@safename"/>Id] = value; }
    }
  </xsl:template>

  <xsl:template name="renderFieldIds">
    private static readonly Sitecore.Data.ID <xsl:value-of select="@safename"/>Id = new Sitecore.Data.ID("<xsl:value-of select="@id"/>");
  </xsl:template>
</xsl:stylesheet>
