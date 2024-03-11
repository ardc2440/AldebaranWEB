async function print(id) {
    var el = document.getElementById(id);
    var win = window.open('', '', 'height=500,width=500');
    var styles = document.createElement('link');
    styles.href = '/css/print.css'; // Ruta absoluta al archivo CSS
    styles.rel = 'stylesheet';
    styles.type = 'text/css';
    win.document.write('<html><head>');
    win.document.head.appendChild(styles);
    win.document.write('</head><body>');
    win.document.write(el.innerHTML);
    win.document.write('</body></html>');
    win.document.close();
    win.print();
}
async function readMoreToggle(id, val) {
    var toggleLink = document.getElementById(id);
    var readMoreElements = document.querySelectorAll('.read-more');
    readMoreElements.forEach(function (element) {
        if (val !== undefined) {
            element.style.display = val ? 'inline' : 'none';
            toggleLink.textContent = val ? 'Ver menos' : 'Ver más';
        } else {
            element.style.display = (element.style.display === 'none') ? 'inline' : 'none';
            toggleLink.textContent = (element.style.display === 'none') ? 'Ver más' : 'Ver menos';
        }
    });    
    //toggleLink.textContent = (toggleLink.textContent === 'Ver más') ? 'Ver menos' : 'Ver más';
}
async function downloadFile(fileName, type, content) {
    const byteArray = Uint8Array.from(atob(content), c => c.charCodeAt(0));
    const blob = new Blob([byteArray], { type: type });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
}
async function getContent(id) {
    var el = document.getElementById(id);
    var html = await fetch('/css/print.css').then(response => {
        if (!response.ok) {
            return "";
        } return response.text()
    }).then(cssText => {
        return '<html><head><style>' + cssText + '</style></head><body>' + el.innerHTML + '</body></html>';
    });
    return html;
}