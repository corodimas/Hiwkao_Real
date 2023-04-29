// Load the content of the first HTML file
var xhr1 = new XMLHttpRequest();
xhr1.open("GET", "index.html", true);
xhr1.onreadystatechange = function() {
    if (xhr1.readyState === 4 && xhr1.status === 200) {
        // Append the content to the empty div
        document.getElementById("main-body").innerHTML = xhr1.responseText;
        
        // Load the content of the second HTML file
        var xhr2 = new XMLHttpRequest();
        xhr2.open("GET", "OrderBox.html", true);
        xhr2.onreadystatechange = function() {
            if (xhr2.readyState === 4 && xhr2.status === 200) {
                // Append the content to the div
                document.getElementById("main-body").innerHTML += xhr2.responseText;
            }
        };
        xhr2.send();
    }
};
xhr1.send();