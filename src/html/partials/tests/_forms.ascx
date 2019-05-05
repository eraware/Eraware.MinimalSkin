<div style="margin:10px;padding:10px;border:1px solid black">
    <pre>This section illustrates the Dnn form patterns</pre>
    <div class="dnnForm" id="form-demo">
        <asp:Label runat="server" CssClass="dnnFormMessage dnnFormInfo" Text="Intro" />
        <div class="dnnFormItem dnnFormHelp dnnClear">
            <p class="dnnFormRequired">
                <asp:Label runat="server" Text="Required Indicator" />
            </p>
        </div>
        <fieldset>
            <div class="dnnFormItem">
                <dnn:Label runat="server" ControlName="NameTextBox" Text="Name" HelpText="Enter your name" />
                <asp:TextBox runat="server" ID="NameTextBox" CssClass="dnnFormRequired" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="NameTextBox" CssClass="dnnFormMessage dnnFormError" Text="Name.Required" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label runat="server" ControlName="DescriptionTextBox" Text="Description" HelpText="Enter a description" />
                <asp:TextBox runat="server" TextMode="MultiLine" ID="DescriptionTextBox" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label runat="server" ControlName="ChoiceDropDown" Text="Choice" HelpText="Choose something" />
                <asp:DropDownList runat="server" ID="ChoiceDropDown">
                    <asp:ListItem Text="-- Make a Choice --" />
                    <asp:ListItem Text="First Choice" />
                    <asp:ListItem Text="Second Choice" />
                </asp:DropDownList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label runat="server" ControlName="RateRadioButtons" Text="Rate" HelpText="Choose another thing" />
                <asp:RadioButtonList runat="server" ID="RateRadioButtons" RepeatDirection="Horizontal" CssClass="dnnFormRadioButtons">
                    <asp:ListItem Text="1" />
                    <asp:ListItem Text="2" />
                    <asp:ListItem Text="3" />
                    <asp:ListItem Text="4" />
                    <asp:ListItem Text="5" />
                </asp:RadioButtonList>
            </div>
            <div class="dnnFormItem">
                <dnn:Label runat="server" ControlName="AgreeCheckbox" Text="Agree" HelpText="Check this box" />
                <asp:CheckBox runat="server" ID="AgreeCheckbox" />
            </div>
            <div class="dnnFormItem">
                <asp:Label runat="server" CssClass="dnnFormLabel" AssociatedControlID="StartDatePicker" Text="Start Date" HelpText="Enter some more things" />
                <asp:textbox runat="server" CssClass="dnnFormInput" ID="StartDatePicker" />
            </div>
        </fieldset>
        <fieldset>
            <div class="dnnFormItem">
                <dnn:Label runat="server" ControlName="DisabledInput" Text="Disabled Input" HelpText="Do not enter anything here" />
                <asp:Textbox runat="server" CssClass="dnnFormInput" Enabled="false" id="DisabledTextBox" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label runat="server" ControlName="DisabledTextArea" Text="Disabled Multiline" HelpText="Do not enter anything here"/>
                <asp:Textbox runat="server" TextMode="MultiLine" CssClass="dnnFormInput" Enabled="false" id="DisabledTextArea" />
            </div>
        </fieldset>
        <ul class="dnnActions dnnClear">
            <li>
                <asp:LinkButton runat="server" CssClass="dnnPrimaryAction" Text="Save" />
            </li>
            <li>
                <asp:HyperLink runat="server" CssClass="dnnSecondaryAction" NavigateUrl="/" Text="Cancel" />
            </li>
            <li>
                <asp:HyperLink runat="server" CssClass="dnnTertiaryAction" NavigateUrl="/" Text="Tertiary"></asp:HyperLink>
            </li>
            <li>
                <asp:HyperLink runat="server" CssClass="dnnSecondaryAction dnnDisabled" Enabled="false" NavigateUrl="/" Text="Disabled"></asp:HyperLink>
            </li>
        </ul>
    </div>
</div>