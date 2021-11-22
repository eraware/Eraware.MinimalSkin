<header>
    <div class="user-controls">
        <div class="wrapper">
            <dnn:Language id="dnnLanguage" runat="server"
                ShowMenu="False"
                ShowLinks="True"
                ItemTemplate='<a href="[URL]">[CULTURE:LanguageNativeName]</a>'
                AlternateTemplate='<a href="[URL]">[CULTURE:LanguageNativeName]</a>'
                SelectedItemTemplate="<span></span>"
                ShowCountry="false"
                />
            <dnn:LOGIN ID="dnnLogin" CssClass="LoginLink" runat="server" LegacyMode="false" />
            <dnn:USER ID="dnnUser" runat="server" LegacyMode="false" />
        </div>
    </div>
    <div class="menu-row wrapper">
        <dnn:logo id="DnnLogo" runat="server" CssClass="dnnLogo" />
        <div class="menu-column">
            <dnn:Menu id="mainMenu"
                runat="server"
                MenuStyle="menus\main"
                NodeSelector="*">
            </dnn:Menu>
            <div class="dnnSearch">
                <dnn:SEARCH ID="dnnSearch" runat="server" ShowSite="false" ShowWeb="false" EnableTheming="true" CssClass="SearchButton" />
            </div>
        </div>
    </div>
</header>