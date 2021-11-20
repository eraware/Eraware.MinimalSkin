<!-- HEAD, SUBHEAD, ETC. EXAMPLE FROM CORE REPOSITORY MODULE --> 
<br />
<table class="Settings" cellspacing="2" cellpadding="2" width="560" summary="Edit XML Design Table"
    border="0"> 
    <tr>
        <td>
            <dnn:SectionHead ID="dshXmlSource" runat="server" IsExpanded="true" IncludeRule="true"
                Text="Xml Source" Section="secXmlSource" CssClass="Head"></dnn:SectionHead>
        </td>
    </tr>
    <tr id="secXmlSource" runat="server">
        <td>
            <p>
                <asp:Label ID="lblXmlSource" resourcekey="lblXmlSource" runat="server" CssClass="normal">In this section, you can define the source of your Xml data. It can be provided 
				as local file or be queried via http using dynamic querystrings.</asp:Label></p>
            <div class="normalbold">
                <asp:Label ID="lblXmlSourceType" resourcekey="lblXmlSourceType" runat="server"> Data Source Type:</asp:Label></div>
            <asp:RadioButtonList ID="rblXmlDataProvider" runat="server" AutoPostBack="true" CssClass="normalbold">
            </asp:RadioButtonList>
            <br />
            <asp:PlaceHolder ID="SourceSettingsPlaceHolder" runat="server"></asp:PlaceHolder>
        </td>
    </tr>
    <tr>
        <td>
            <br />
            <dnn:SectionHead ID="dshXslTrans" runat="server" IsExpanded="true" IncludeRule="true"
                Text="XSL Transform" Section="secXslTrans" CssClass="Head"></dnn:SectionHead>
        </td>
    </tr>
    <tr id="secXslTrans" runat="server">
        <td>
            <div class="normalbold">
                <asp:Label ID="Label1" resourcekey="lblXmlRenderingType" runat="server"> Data Source Type:</asp:Label></div>
            <asp:RadioButtonList ID="rblXmlRendering" runat="server" AutoPostBack="true" CssClass="normalbold">
            </asp:RadioButtonList>
            <br />
            <asp:PlaceHolder ID="XmlRenderingPlaceHolder" runat="server"></asp:PlaceHolder>
        </td>
    </tr>
    <tr>
        <td>
            <br />
            <dnn:SectionHead ID="dshAdvanced" runat="server" IsExpanded="false" IncludeRule="true"
                Text="Advanced" Section="secAdvanced" CssClass="Head"></dnn:SectionHead>
        </td>
    </tr>
    <tr id="secAdvanced" runat="server">
        <td>
            <table id="tblAdvanced" runat="server">
                <tr>
                    <td valign="top" colspan="2">
                        <asp:Label ID="lblOutput" runat="server" resourcekey="lblOutput" Text="The Xml Module will usually render its output as html inside the module. However it is possible to create downloads too."
                            CssClass="normal" />
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="SubHead" valign="top">
                        <dnn:Label ID="plOutput" Suffix=":" ControlName="rblOutput" runat="server" Text="Return result:" />
                    </td>
                    <td>
                        <asp:RadioButtonList ID="rblOutput" runat="server" CssClass="normalBold">
                            <asp:ListItem Value="Inline" Selected="True" resourcekey="OutputInline">inside module</asp:ListItem>
                            <asp:ListItem Value="Link" resourcekey="OutputLink">as link to a file stream</asp:ListItem>
                            <asp:ListItem Value="Response" resourcekey="OutputResponse">as  file stream</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="SubHead" valign="top">
                        <dnn:Label ID="plContentType" Suffix=":" ControlName="rblContentType" runat="server"
                            Text="Content type:" />
                    </td>
                    <td>
                        <asp:RadioButtonList ID="rblContentType" runat="server" CssClass="normalBold">
                            <asp:ListItem Value="xml|text/xml" Selected="True">*.xml (text/xml)</asp:ListItem>
                            <asp:ListItem Value="txt|text/html">*.htm (text/html)</asp:ListItem>
                            <asp:ListItem Value="csv|text/comma-separated-values">*.csv (text/comma-separated-values)</asp:ListItem>
                            <asp:ListItem Value="txt|text/plain">*.txt (text/plain)</asp:ListItem>
                        </asp:RadioButtonList></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <hr />
                        <asp:Label ID="lblSearch" runat="server" resourcekey="lblSearch" CssClass="normal">The Output of the Xml Module is not searchable in DotNetNuke Search by default. However, if you want to and your setup doesn't depend on dynamic parameters you can switch own indexing.</asp:Label>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td valign="top" class="SubHead">
                        <dnn:Label ID="plIndexRun" Suffix=":" ControlName="rblIndexRun" runat="server" Text="Allow index run:" />
                    </td>
                    <td>
                        <span class="normalBold">
                            <asp:RadioButtonList ID="rblIndexRun" runat="server" CssClass="normalBold">
                                <asp:ListItem Value="Never" Selected="True" resourcekey="IndexRunNever">never (search is disabled)</asp:ListItem>
                                <asp:ListItem Value="NextRun" resourcekey="IndexRunNextRun">only on next run</asp:ListItem>
                                <asp:ListItem Value="Always" resourcekey="IndexRunAlways">always</asp:ListItem>
                                <asp:ListItem Value="OncePerHour" resourcekey="IndexRunOncePerHour">once per hour</asp:ListItem>
                                <asp:ListItem Value="OncePerDay" resourcekey="IndexRunOncePerDay">once per day</asp:ListItem>
                            </asp:RadioButtonList>
                            <asp:Label ID="lblDynamicParameter" runat="server" Visible="False" resourcekey="lblDynamicParameter">Index run is disabled as there are dynamic parameters for either query string or xsl args.</asp:Label>
                            <br />
                            <br />
                            <asp:LinkButton CssClass="CommandButton" ID="cmdClearSearchIndex" runat="server"
                                resourcekey="cmdClearSearchIndex" BorderStyle="none" CausesValidation="False">Clear Search Index</asp:LinkButton>
                        </span>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <hr />
                        <asp:Label ID="lblEnableByQuerystring" runat="server" resourcekey="lblEnableByQuerystring"
                            CssClass="normal">For some use cases it is necessary that the module only runs if the request contains a defined querystring parameter/ value pair.</asp:Label>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td valign="top" class="NormalBold">
                        <dnn:Label ID="plEnableParam" Suffix=":" ControlName="txtEnableParam" runat="server"
                            Text="Querystring param:" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtEnableParam" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td valign="top" class="NormalBold">
                        <dnn:Label ID="plEnableValue" Suffix=":" ControlName="txtEnableValue" runat="server"
                            Text="Querystring value:" />
                    </td>
                    <td>
                        <asp:TextBox ID="txtEnableValue" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="SubHead" valign="top">
                    </td>
                    <td>
                        <asp:LinkButton class="CommandButton" ID="cmdClearEnableByParam" runat="server" resourcekey="cmdClearEnableByParam"
                            BorderStyle="none" CausesValidation="False">Clear/ Disable</asp:LinkButton></td>
                </tr>
            </table>
            <p>
            </p>
            <hr />
            <p>
            </p>
        </td>
    </tr>
</table>
<asp:LinkButton class="CommandButton" ID="cmdUpdate" runat="server" Text="Update"
    BorderStyle="none" resourcekey="cmdUpdate"></asp:LinkButton>&nbsp;
<asp:LinkButton class="CommandButton" ID="cmdCancel" runat="server" Text="Cancel"
    BorderStyle="none" resourcekey="cmdCancel" CausesValidation="False"></asp:LinkButton>