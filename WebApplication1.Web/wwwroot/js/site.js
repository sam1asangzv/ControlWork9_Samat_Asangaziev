(() => {
    const toastElement = document.getElementById("wallet-toast");
    const toastMessage = document.getElementById("wallet-toast-message");
    const networkError = document.body.dataset.networkError;
    const decimalSeparator = document.body.dataset.decimalSeparator || ".";

    function showToast(message, succeeded) {
        toastMessage.textContent = message;
        toastElement.classList.remove("text-bg-success", "text-bg-danger");
        toastElement.classList.add(succeeded ? "text-bg-success" : "text-bg-danger");
        bootstrap.Toast.getOrCreateInstance(toastElement, { delay: 4500 }).show();
    }

    const serverMessage = document.getElementById("server-toast-message")?.dataset.message;
    const savedMessage = sessionStorage.getItem("wallet-toast");
    if (savedMessage) {
        sessionStorage.removeItem("wallet-toast");
        showToast(savedMessage, true);
    } else if (serverMessage) {
        showToast(serverMessage, true);
    }

    document.querySelectorAll("form[data-wallet-ajax]").forEach(form => {
        form.addEventListener("submit", async event => {
            event.preventDefault();
            if (!form.reportValidity()) return;

            const submitButton = form.querySelector('button[type="submit"]');
            submitButton.disabled = true;
            try {
                const formData = new FormData(form);
                const amount = formData.get("Amount");
                if (amount !== null) formData.set("Amount", amount.replace(".", decimalSeparator));
                const token = formData.get("__RequestVerificationToken");
                const response = await fetch(form.action, {
                    method: "POST",
                    body: formData,
                    headers: { "RequestVerificationToken": token, "X-Requested-With": "XMLHttpRequest" }
                });
                const result = await response.json();
                if (result.success) {
                    sessionStorage.setItem("wallet-toast", result.message);
                    window.location.reload();
                } else {
                    showToast(result.message, false);
                }
            } catch {
                showToast(networkError, false);
            } finally {
                submitButton.disabled = false;
            }
        });
    });
})();
