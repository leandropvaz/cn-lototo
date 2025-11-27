window.adminlteToasts = {
    show: function (type, title, message) {

        var cssClass = "bg-info";
        var icon = "fas fa-info-circle";

        if (type === "success") {
            cssClass = "bg-success";
            icon = "fas fa-check-circle";
        }
        else if (type === "error") {
            cssClass = "bg-danger";
            icon = "fas fa-exclamation-triangle";
        }

        $(document).Toasts('create', {
            class: cssClass,
            title: title,
            autohide: true,
            delay: 3500,
            icon: icon,
            body: message
        });
    }
}
