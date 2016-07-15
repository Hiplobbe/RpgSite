$(function () {
    tabSelect('#wikiTab');
});
function tabSelect(tab) {
    $(".jsTabs").hide();
    $(tab).toggle();
}
function updatePartial(link, contentDiv) {
    $(contentDiv).load(link);
}
//# sourceMappingURL=Chat.js.map