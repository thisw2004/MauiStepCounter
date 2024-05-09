// windowsNotifications.js

function showWindowsToastNotification(title, message) {
    if (Windows && Windows.UI && Windows.UI.Notifications) {
        var template = Windows.UI.Notifications.ToastTemplateType.toastText02;
        var toastXml = Windows.UI.Notifications.ToastNotificationManager.getTemplateContent(template);
        var toastTextElements = toastXml.getElementsByTagName("text");
        toastTextElements[0].appendChild(toastXml.createTextNode(title));
        toastTextElements[1].appendChild(toastXml.createTextNode(message));

        var toast = new Windows.UI.Notifications.ToastNotification(toastXml);
        var toastNotifier = Windows.UI.Notifications.ToastNotificationManager.createToastNotifier();
        toastNotifier.show(toast);
    }
}
