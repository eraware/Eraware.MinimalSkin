<div style="margin:10px;padding:10px;border: 1px solid black;">
    <pre>This section illustrates the use of jQuery UI Tabs and the tabs demo in the UX/UI guide</pre>
    <div class="dnnForm" id="tabs-demo">
        <ul class="dnnAdminTabNav">
            <li><a href="#ChristopherColumbus">Christopher Columbus</a></li>
            <li><a href="#IsaacNewton">Isaac Newton</a></li>
            <li><a href="#JohannesGutenberg">Johannes Gutenberg</a></li>
        </ul>
        <div id="ChristopherColumbus" class="dnnClear">
            <img src="https://picsum.photos/200/300.jpg?1" alt="Christopher Columbus" width="32%" class="dnnLeft" />
            <div class="dnnRight" style="width:62%;margin-left:2%">
                <h1>Christopher Columbus</h1>
                <p>Italian navigator, colonizer and explorer whose voyages led to general European awareness of the American continents.</p>
            </div>
        </div>
        <div id="IsaacNewton" class="dnnClear">
            <img src="https://picsum.photos/200/300.jpg?2" alt="Isaac Newton" width="32%" class="dnnLeft" />
            <div class="dnnRight" style="width:62%;margin-left:2%">
                <h1>Isaac Newton</h1>
                <p>English physicist, mathematician, astronomer, natural philosopher, alchemist, and theologian. His law of universal gravitation and three laws of motion laid the groundwork for classical mechanics.</p>
            </div>
        </div>
        <div id="JohannesGutenberg" class="dnnClear">
            <img src="https://picsum.photos/200/300.jpg?3" alt="Johannes Gutenberg" width="32%" class="dnnLeft" />
            <div class="dnnRight" style="width:62%;margin-left:2%">
                <h1>Johannes Gutenberg</h1>
                <p>German printer who invented the mechanical printing press.</p>
            </div>
        </div>
    </div>
</div>

<script type="text/javascript">
    jQuery(function ($) {
        $('#tabs-demo').dnnTabs();
    });
</script>