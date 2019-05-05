<div style="margin:10px;padding:10px;border:1px solid black;min-height:300px;">
    <pre>This section illustrates the login control</pre>
    <div id="dnn_ctr404_ModuleContent" class="DNNModuleContent ModAuthenticationC">
	<div id="dnn_ctr404_Login_UP">
		
<div class="dnnForm dnnLogin dnnClear">
    <div id="dnn_ctr404_Login_pnlLogin">
			
        <div class="loginContent">
            
            <div id="dnn_ctr404_Login_pnlLoginContainer" class="LoginPanel">
				<div id="dnn_ctr404_Login_DNN">
<div class="dnnForm dnnLoginService dnnClear">
    <div class="dnnFormItem">
		<div class="dnnLabel">
			<label for="dnn_ctr404_Login_Login_DNN_txtUsername" id="dnn_ctr404_Login_Login_DNN_plUsername" class="dnnFormLabel">Username:</label>
		</div>        
        <input name="dnn$ctr404$Login$Login_DNN$txtUsername" type="text" id="dnn_ctr404_Login_Login_DNN_txtUsername" autocomplete="off">
    </div>
    <div class="dnnFormItem">
		<div class="dnnLabel">
			<label for="dnn_ctr404_Login_Login_DNN_txtPassword" id="dnn_ctr404_Login_Login_DNN_plPassword" class="dnnFormLabel">Password:</label>
		</div>
        <input name="dnn$ctr404$Login$Login_DNN$txtPassword" type="password" id="dnn_ctr404_Login_Login_DNN_txtPassword" autocomplete="off" style="background-image: url(&quot;data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAACIUlEQVQ4EX2TOYhTURSG87IMihDsjGghBhFBmHFDHLWwSqcikk4RRKJgk0KL7C8bMpWpZtIqNkEUl1ZCgs0wOo0SxiLMDApWlgOPrH7/5b2QkYwX7jvn/uc//zl3edZ4PPbNGvF4fC4ajR5VrNvt/mo0Gr1ZPOtfgWw2e9Lv9+chX7cs64CS4Oxg3o9GI7tUKv0Q5o1dAiTfCgQCLwnOkfQOu+oSLyJ2A783HA7vIPLGxX0TgVwud4HKn0nc7Pf7N6vV6oZHkkX8FPG3uMfgXC0Wi2vCg/poUKGGcagQI3k7k8mcp5slcGswGDwpl8tfwGJg3xB6Dvey8vz6oH4C3iXcFYjbwiDeo1KafafkC3NjK7iL5ESFGQEUF7Sg+ifZdDp9GnMF/KGmfBdT2HCwZ7TwtrBPC7rQaav6Iv48rqZwg+F+p8hOMBj0IbxfMdMBrW5pAVGV/ztINByENkU0t5BIJEKRSOQ3Aj+Z57iFs1R5NK3EQS6HQqF1zmQdzpFWq3W42WwOTAf1er1PF2USFlC+qxMvFAr3HcexWX+QX6lUvsKpkTyPSEXJkw6MQ4S38Ljdbi8rmM/nY+CvgNcQqdH6U/xrYK9t244jZv6ByUOSiDdIfgBZ12U6dHEHu9TpdIr8F0OP692CtzaW/a6y3y0Wx5kbFHvGuXzkgf0xhKnPzA4UTyaTB8Ph8AvcHi3fnsrZ7Wore02YViqVOrRXXPhfqP8j6MYlawoAAAAASUVORK5CYII=&quot;); background-repeat: no-repeat; background-attachment: scroll; background-size: 16px 18px; background-position: 98% 50%;">
    </div>
    
    
    <div class="dnnFormItem">
        <span id="dnn_ctr404_Login_Login_DNN_lblLogin" class="dnnFormLabel"></span>
        <a id="dnn_ctr404_Login_Login_DNN_cmdLogin" title="Login" class="dnnPrimaryAction" href="javascript:__doPostBack('dnn$ctr404$Login$Login_DNN$cmdLogin','')">Login</a>
		<a id="dnn_ctr404_Login_Login_DNN_cancelLink" class="dnnSecondaryAction" causesvalidation="false" href="//localhost:3000/Events">Cancel</a>
        
    </div>
	<div class="dnnFormItem">
		<span id="dnn_ctr404_Login_Login_DNN_lblLoginRememberMe" class="dnnFormLabel"></span>
		<span class="dnnLoginRememberMe"><input id="dnn_ctr404_Login_Login_DNN_chkCookie" type="checkbox" name="dnn$ctr404$Login$Login_DNN$chkCookie" style="position: absolute; z-index: -1; opacity: 0;"><span class="dnnCheckbox"><span class="mark"><img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAMAAAAoyzS7AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAAZQTFRFAAAAAAAApWe5zwAAAAF0Uk5TAEDm2GYAAAAMSURBVHjaYmAACDAAAAIAAU9tWeEAAAAASUVORK5CYII=" alt="checkbox"></span></span><label for="dnn_ctr404_Login_Login_DNN_chkCookie" class="dnnBoxLabel">Remember Login</label></span>
	</div>
    <div class="dnnFormItem">
        <span class="dnnFormLabel">&nbsp;</span>
        <div class="dnnLoginActions">
            <ul class="dnnActions dnnClear">
                <li id="dnn_ctr404_Login_Login_DNN_liRegister"><a id="dnn_ctr404_Login_Login_DNN_registerLink" class="dnnSecondaryAction" onclick="return dnnModal.show('//localhost:3000/Register?returnurl=http%253a%252f%252fdnn932clean.localtest.me%252fEvents&amp;popUp=true',/*showReturn*/true,600,950,true,'')" href="//localhost:3000/Register?returnurl=http%3a%2f%2fdnn932clean.localtest.me%2fEvents">Register</a></li>                
                <li id="dnn_ctr404_Login_Login_DNN_liPassword"><a id="dnn_ctr404_Login_Login_DNN_passwordLink" class="dnnSecondaryAction" onclick="return dnnModal.show('//localhost:3000/Events/ctl/SendPassword?returnurl=http%253a%252f%252fdnn932clean.localtest.me%252fEvents&amp;popUp=true',/*showReturn*/true,300,650,true,'')" href="//localhost:3000/Events/ctl/SendPassword?returnurl=http%3a%2f%2fdnn932clean.localtest.me%2fEvents">Reset Password</a></li>
            </ul>
        </div>
    </div>
</div>

    <script type="text/javascript">
        /*globals jQuery, window, Sys */
        (function ($, Sys) {
            const disabledActionClass = "dnnDisabledAction";
            const actionLinks = $('a[id^="dnn_ctr404_Login_Login_DNN"]');
            function isActionDisabled($el) {
                return $el && $el.hasClass(disabledActionClass);
            }
            function disableAction($el) {
                if ($el == null || $el.hasClass(disabledActionClass)) {
                    return;
                }
                $el.addClass(disabledActionClass);
            }
            function enableAction($el) {
                if ($el == null) {
                    return;
                }
                $el.removeClass(disabledActionClass);
            }
            function setUpLogin() {                
                $.each(actionLinks || [], function (index, action) {
                    var $action = $(action);
                    $action.click(function () {
                        var $el = $(this);
                        if (isActionDisabled($el)) {
                            return false;
                        }
                        disableAction($el);
                    });
                });
            }
		
            $(document).ready(function () {
                $(document).on('keydown', '.dnnLoginService', function (e) {
                    if ($(e.target).is('input:text,input:password') && e.keyCode === 13) {
                        var $loginButton = $('#dnn_ctr404_Login_Login_DNN_cmdLogin');
                        if (isActionDisabled($loginButton)) {
                            return false;
                        }
                        disableAction($loginButton);
                        window.setTimeout(function () { eval($loginButton.attr('href')); }, 100);
                        e.preventDefault();
                        return false;
                    }
                });

                setUpLogin();
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                    $.each(actionLinks || [], function (index, item) {
                        enableAction($(item));
                    });
                    setUpLogin();
                });
            });
        }(jQuery, window.Sys));
    </script>

</div>
			</div>
            <div class="dnnSocialRegistration">
                <div id="socialControls">
                    <ul class="buttonList">
                        
                    </ul>
                </div>
            </div>
        </div>
    
		</div>
    
    
    
    
</div>
	</div><div id="dnn_ctr404_Login_UP_Prog" style="display:none;" role="status" aria-hidden="true">
		<div class="dnnLoading dnnPanelLoading"></div>
	</div>
</div>
</div>