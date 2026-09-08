window.aldebaranNotifications = {

    async requestPermission() {

        if (!("Notification" in window)) return false;

        if (Notification.permission === "granted") return true;

        const permission = await Notification.requestPermission();

        return permission === "granted";
    },

    show(title, message) {

        if (!("Notification" in window)) return;

        if (Notification.permission !== "granted") return;

        new Notification(title, { body: message, icon: '/favicon.ico' });
    }
};