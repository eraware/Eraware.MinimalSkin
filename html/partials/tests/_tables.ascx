<div style="margin:10px;padding:10px;border: 1px solid black;">
    <pre>This section illustrates the use of dnnTableDisplay, this should be moved into the only 2 modules that use it, Subscriptions and Search Results</pre>
    <div class="dnnSubscriptions" id="dnnSubscriptions">
<asp:Panel runat="server" ID="ScopeWrapper" CssClass="dnnClear">
    <div class="activities-list-container dnnForm">        
        <h2 id="dnnSitePanel-ContentItem" class="dnnFormSectionHead"><a href="">Manage Subscriptions</a></h2>
        <fieldset data-bind=" with: $root.subscriptionsViewModel">
            
            <div class="dnnFormItem">
                <div class="dnnFormMessage">
                    <asp:label ID="lblSubscriptionsHelp" runat="server" resourcekey="lblSubscriptions.Help"/>
                </div>         
                <div class="subscription-table-container">
                    <div class="loading-panel" data-bind="visible: isLoading"></div>

                    <table class="dnnTableDisplay" id="subscription-table">
                        <colgroup>
                            <col class="subscriptions-col-subscribed-description"/>
                            <col class="subscriptions-col-subscription-type"/>                    
                            <col class="subscriptions-col-action"/>
                        </colgroup>
                        <thead>
                            <tr class="dnnGridHeader">
                                <th class="subscriptions-col-subscribed-description">
                                    <span class="sortable" data-bind="click: sortByDescription">Subscribed Description</span>
                                    <span class="sortArrow" data-bind="click: sortByDescription, css: sortCssDescription"></span>
                                </th>
                                <th class="subscriptions-col-subscription-type">
                                    <span class="sortable" data-bind="click: sortBySubscriptionType">Subscription Type</span>
                                    <span class="sortArrow" data-bind="click: sortBySubscriptionType, css: sortCssSubscriptionType"></span>
                                </th>
                                <th class="subscriptions-col-action">
                                    <span>Action</span>
                                </th>
                            </tr>
                        </thead>
                        <tfoot>
                            <tr>
                                <td colspan="3">
      
                                    <div class="subscriptions-page-size">
                                        Items Per Page
                                        <select data-bind="value: pageSize, event: { change: function () { changePage(0) } }" aria-label="Page Size">
                                            <option value="10" selected="selected">10</option>
                                            <option value="25">25</option>
                                            <option value="50">50</option>                                        
                                        </select>
                                    </div>
                                                          
                                    <div class="subscriptions-pager" data-bind="if: pages().length > 1">
                                        <a href="#" data-bind="click: function () { changePage(0) }, css: { disabled: currentPage() == 0 }">First</a>
                                        <a href="#" data-bind="click: function () { changePage(currentPage() - 1) }, css: { disabled: currentPage() == 0 }">Prev</a>
                                        <ul data-bind="foreach: pages">
                                            <li><a href="#" data-bind="click: function () { $parent.changePage($data - 1) }, text: $data, css: { currentPage: $data - 1 == $parent.currentPage() }"></a></li>
                                        </ul>
                                        <a href="#" data-bind="click: function () { changePage(currentPage() + 1) }, css: { disabled: currentPage() == lastPage() - 1 }">Next</a>
                                        <a href="#" data-bind="click: function () { changePage(lastPage() - 1) }, css: { disabled: currentPage() == lastPage() - 1 }">Last</a>
                                    </div>
                            
                                    <div class="subscriptions-count" data-bind="visible: totalCount() > 0, text: totalItemsText"></div>

                                </td>
                            </tr>
                        </tfoot>
                        <tbody data-bind="foreach: results">
                            <tr>
                                <td>
                                    <span data-bind="text: description"></span>
                                </td>                        
                                <td> 
                                    <span data-bind="text: subscriptionType"></span>
                                </td>
                                <td>
                                    <a href="#" data-bind="click: $root['delete']">
                                        <img src='<%= ResolveUrl("~/DesktopModules/CoreMessaging/Images/reply.png") %>' alt="Unsubscribe"/>
                                    </a>
                                </td>
                            </tr>
                        </tbody>
                    </table>                    
                </div>
                
                <div class="dnnClear" style="display:none;" id="divUnsubscribed">
                    <div class="dnnFormMessage dnnFormSuccess"><span>Unsubscribed</span></div>
                </div>
            </div>
        </fieldset>
        <h2 id="dnnSitePanel-Subscriptions" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded">Email Delivery Schedule</a></h2>
        <fieldset id="fsFrequency">
            <div class="dnnFormItem">
                <dnn:label id="lblNotificationFreq" runat="server" controlname="ddlNotify" />
                <select data-bind="options: $root.frequencyOptions, optionsValue: 'value', optionsText: 'text', value: $root.notifyFrequency" id="ddlNotify"></select>
            </div>     
            <div class="dnnFormItem">
                <dnn:label id="lblMessageFreq" runat="server" controlname="ddlMsg" />
                <select data-bind="options: $root.frequencyOptions, optionsValue: 'value', optionsText: 'text', value: $root.msgFrequency" id="ddlMsg"></select>
            </div>
            <div class="dnnClear">
                <ul class="dnnActions dnnLeft">
                    <li><a href="#" class="dnnPrimaryAction" data-bind="click: save">Save</a></li>
                </ul>
            </div>
            <div class="dnnClear" style="display:none;" id="divUpdated">
                <div class="dnnFormMessage dnnFormSuccess"><span>PrefSaved</span></div>
            </div>
        </fieldset>
    </div>
</asp:Panel>
</div>
<script type="text/javascript">
    $(document).ready(function() {
        $('#dnnSubscriptions').dnnPanels();
    });
</script>

<hr />
<table class="dnnSearchResult-hint-tbl dnnTableDisplay">

                
				<tr>
                    <th style="width: 150px"><span>Type</span>
                    </th>
                    <th style="width: 250px"><span>Example</span>
                    </th>
                    <th><span>Notes</span>
                    </th>
				</tr>
         

                <tr>
                    <td>Fuzzy</td>
                    <td>kettle<b>~</b></td>
                    <td>Contain terms that are close to the word <i>kettle</i>, such as <i>cattle</i></td>
                </tr>

                <tr>
                    <td>Wild</td>
                    <td>cat*</td>
                    <td>Contain terms that begin with <i>cat</i>, such as <i>category</i> and the extact term <i>cat</i> itself</td>
                </tr>

                <tr>
                    <td>Exact-Single</td>
                    <td>orange</td>
                    <td>Contain the term <i>orange</i></td>
                </tr>

                <tr>
                    <td>Exact-Phrase</td>
                    <td>"dnn is awesome"</td>
                    <td>Contain the exact phase <i>dnn is awesome</i></td>
                </tr>

                <tr>
                    <td>OR</td>
                    <td>orange bike</td>
                    <td>Contain the term <i>orange</i> or <i>bike</i>, or both. <b><i>OR</i>, if used, must be in uppercase</b></td>
                </tr>

                <tr>
                    <td></td>
                    <td>orange <b>OR</b> bike</td>
                    <td></td>
                </tr>

                <tr>
                    <td>AND</td>
                    <td>orange <b>AND</b> bike</td>
                    <td>Contain both <i>orange</i> and <i>bike</i>. <b><i>AND</i> must be in uppercase</b></td></td>
                </tr>

                <tr>
                    <td>Combo</td>
                    <td>(agile <b>OR</b> extreme) <b>AND</b> methodology</td>
                    <td>Contain <i>methodology</i> and must also contain <i>agile</i> and/or <i>extreme</i>
                </tr>
            </table>

</div>