<div style="margin:10px;padding:10px;border: 1px solid black;">
    <pre>This section illustrates the jQuery Resizable components</pre>
    <style type="text/css">
        #resizable { width: 150px; height: 150px; padding: 0.5em; }
        #resizable h3 { text-align: center; margin: 0;}
    </style>

    <div class="dnnForm">
        <div id="resizable" class="ui-widget-content" style="border: 1px solid black;">
            <h3 class="ui-widget-header">Resizable</h3>
        </div>
    </div>

    <script type="text/javascript">
        $(document).ready(function(){
            $('#resizable').resizable();
        });
    </script>
</div>