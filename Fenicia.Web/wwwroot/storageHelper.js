window.storageHelper = {
    get: function (key) {
        try {
            var value = localStorage.getItem(key);
            console.log(`[storageHelper] get ${key}: ${value ? 'found' : 'not found'}`);
            return value;
        } catch (e) {
            console.error(`[storageHelper] get error: ${e.message}`);
            return null;
        }
    },
    set: function (key, value, hours) {
        try {
            localStorage.setItem(key, value);
            console.log(`[storageHelper] set ${key}: ${value.substring(0, 20)}...`);
        } catch (e) {
            console.error(`[storageHelper] set error: ${e.message}`);
        }
    },
    remove: function (key) {
        try {
            localStorage.removeItem(key);
            console.log(`[storageHelper] removed ${key}`);
        } catch (e) {
            console.error(`[storageHelper] remove error: ${e.message}`);
        }
    },

    setCookie: function (name, value, days) {
        try {
            var expires = "";
            if (days) {
                var date = new Date();
                date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                expires = "; expires=" + date.toUTCString();
            }
            document.cookie = name + "=" + (value || "") + expires + "; path=/; SameSite=Lax";
            console.log("[storageHelper] setCookie " + name + ": " + value);
        } catch (e) {
            console.error("[storageHelper] setCookie error: " + e.message);
        }
    },

    getCookie: function (name) {
        try {
            var nameEQ = name + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) === ' ') c = c.substring(1, c.length);
                if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
            }
            console.log("[storageHelper] getCookie " + name + ": not found");
            return null;
        } catch (e) {
            console.error("[storageHelper] getCookie error: " + e.message);
            return null;
        }
    },

    removeCookie: function (name) {
        try {
            document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
            console.log("[storageHelper] removeCookie " + name);
        } catch (e) {
            console.error("[storageHelper] removeCookie error: " + e.message);
        }
    }
};

console.log("[storageHelper] Loaded successfully");
