function toggleButtons() {
    if (document.getElementById("wikiTextArea").hasAttribute("readonly")) {
        document.getElementById("wikiTextArea").removeAttribute("readonly");
    }
    else {
        document.getElementById("wikiTextArea").setAttribute("readonly", "true");
    }
    //TODO: Edit the text inte the textarea, so there are links to the other entries.
    $("#wikiEdit").toggle();
    $("#wikiSubmit").toggle();
}
//# sourceMappingURL=Wiki.js.map