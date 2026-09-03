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
    }
};

console.log("[storageHelper] Loaded successfully");
