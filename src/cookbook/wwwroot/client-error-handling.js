window.addEventListener("load", () => {
    // Handle HTMX errors
    htmx.on("htmx:responseError", (event) => {
        console.error("HTMX response error:", event.detail);
        if (event.detail.xhr.status >= 400) {
            window.location = window.location.origin + "/statuscode/" + event.detail.xhr.status;
        }
    });

    // Handle HTMX errors on request
    htmx.on("htmx:sendError", (event) => {
        console.error("HTMX send error:", event.detail);
    });
});

