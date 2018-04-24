<?xml version="1.0" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:wix="http://schemas.microsoft.com/wix/2006/wi">
  
  <!-- Copy all attributes and elements to the output. -->
  <xsl:template match="@*|*">
    <xsl:copy>
      <xsl:apply-templates select="@*" />
      <xsl:apply-templates select="*" />
    </xsl:copy>
  </xsl:template>
  <xsl:output method="xml" indent="yes" />

  <!-- Search directories for the components that will be removed. -->
  <xsl:key name="bin-search" match="wix:Directory[@Name = 'bin']" use="descendant::wix:Component/@Id" />
  <xsl:key name="obj-search" match="wix:Directory[@Name = 'obj']" use="descendant::wix:Component/@Id" />
  
  <!-- Remove directories. -->
  <xsl:template match="wix:Directory[@Name='bin']" />
  <xsl:template match="wix:Directory[@Name='obj']" />
  
  <!-- Remove componentsrefs referencing components in those directories. -->
  <xsl:template match="wix:ComponentRef[key('bin-search', @Id)]" />
  <xsl:template match="wix:ComponentRef[key('obj-search', @Id)]" />
  
  <!-- Individual components to be removed. -->
  <xsl:key name="pdb-search"
           match="wix:Component/wix:File['.pdb' = substring(@Source, string-length(@Source) - string-length('.pdb') + 1)]"
           use="parent::wix:Component/@Id" />
  <xsl:key name="exe-search"
           match="wix:Component/wix:File['.exe' = substring(@Source, string-length(@Source) - string-length('.exe') + 1)]"
           use="parent::wix:Component/@Id" />
  
  <!-- Remove the individual components. -->
  <xsl:template match="wix:Component[key('pdb-search', @Id)]" />
  <xsl:template match="wix:Component[key('exe-search', @Id)]" />
  
  <!-- Remove componentsrefs referencing the individual components.-->
  <xsl:template match="wix:ComponentRef[key('pdb-search', @Id)]" />
  <xsl:template match="wix:ComponentRef[key('exe-search', @Id)]" />
  
</xsl:stylesheet>