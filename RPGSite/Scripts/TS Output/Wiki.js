/// <reference path="../typings/jquery/jquery.d.ts" />
$(document).ready(function () {
    wikiLinks(false);
});
function toggleButtons() {
    if (!$("#wikiTextArea").is(':visible')) {
        wikiLinks(true);
    }
    else {
        wikiLinks(false);
    }
    $("#wikiTextDiv").toggle();
    $("#wikiTextArea").toggle();
    $("#wikiEdit").toggle();
    $("#wikiSubmit").toggle();
}
//Makes all the [!] blocks into links in view mode or makes the links into [!] while in edit mode.
function wikiLinks(editMode) {
    if (editMode == false) {
        var regexedit = $("#wikiTextArea").val()
            .replace(/\[!(\w+)\]/g, '<a href="Wiki/$1">$1</a>')
            .replace(/\n/g, '<br/>');
        $("#wikiTextDiv").html(regexedit);
    }
}
//# sourceMappingURL=Wiki.js.map