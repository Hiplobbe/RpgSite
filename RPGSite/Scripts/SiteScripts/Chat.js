$(function () {
    tabSelect('#wikiTab');
});
function tabSelect(tab) {
    $(".tabcontent").hide();
    $(tab).toggle();
}
function updatePartial(link, contentDiv) {
    $(contentDiv).load(link);
}
//# sourceMappingURL=Chat.js.map