window.cookieHelper = {
    set: function (name, value, hours) {
        console.log(`[cookieHelper] Setting cookie: ${name}`);
        var expires = "";
        if (hours) {
            var date = new Date();
            date.setTime(date.getTime() + (hours * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + encodeURIComponent(value) + expires + "; path=/; SameSite=Strict";
    },
    get: function (name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) === ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) === 0) {
                var val = decodeURIComponent(c.substring(nameEQ.length, c.length));
                console.log(`[cookieHelper] Found cookie ${name}: ${val.substring(0, 20)}...`);
                return val;
            }
        }
        console.log(`[cookieHelper] Cookie ${name} not found`);
        return null;
    },
    remove: function (name) {
        document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
    }
};

console.log("[cookieHelper] Loaded successfully");
