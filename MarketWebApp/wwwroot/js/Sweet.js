function ShopCartDelete() {
    var deleteLinks = document.querySelectorAll('.delete-link');

    deleteLinks.forEach(function (deleteLink) {
        deleteLink.addEventListener('click', function (e) {
            e.preventDefault();
            var itemId = this.getAttribute('data-id');
            Swal.fire({
                title: "Are you sure?",
                text: "You won't be able to revert this!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Yes, delete it!"
            }).then((result) => {
                if (result.isConfirmed) {
                    Swal.fire({
                        title: "Deleted!",
                        text: "Your ShoppingCart has been deleted.",
                        icon: "success",

                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.href = "/ShoppingCart/Delete?id=" + itemId;
                        }
                    });
                }
            });
        });
    });
}

function ADDCart(Form) {
    event.preventDefault();
    var isAuthenticated = document.getElementById('auth-status').getAttribute('data-authenticated');
    if (isAuthenticated == "false") {
        window.location.href = '/Identity/Account/Login'; 
        return false;
    }
    fetch(Form.action, {
        method: Form.method,
        body: new FormData(Form)
    })
        .then(response => {
            if (response.ok) {
                // Show Toast notification
                const Toast = Swal.mixin({
                    toast: true,
                    position: "top-end",
                    showConfirmButton: false,
                    timer: 3000,
                    timerProgressBar: true,
                    didOpen: (toast) => {
                        toast.onmouseenter = Swal.stopTimer;
                        toast.onmouseleave = Swal.resumeTimer;
                    }
                });
                Toast.fire({
                    icon: "success",
                    title: "Product added to cart successfully"
                });

                Form.reset();
            } else {
                console.error('Error:', response.statusText);
            }
        })
        .catch(error => {
            console.error('Error:', error);
        });
    return false;

};

function ADDWish(form) {
    event.preventDefault();

    fetch(form.action, {
        method: form.method,
        body: new FormData(form)
    })
        .then(response => {
            if (response.ok) {
                const Toast = Swal.mixin({
                    toast: true,
                    position: "top-end",
                    showConfirmButton: false,
                    timer: 3000,
                    timerProgressBar: true,
                    didOpen: (toast) => {
                        toast.onmouseenter = Swal.stopTimer;
                        toast.onmouseleave = Swal.resumeTimer;
                    }
                });
                Toast.fire({
                    icon: "success",
                    title: "Product added to WishList successfully"
                });

                // Reset the form if needed
                form.reset();
            } else {
                // Handle errors if any
                console.error('Error:', response.statusText);
            }
        })
        .catch(error => {
            console.error('Error:', error);
        });

    return false;
}
function attachDeleteListeners() {
    var deleteLinks = document.querySelectorAll('.delete-link');

    deleteLinks.forEach(function (deleteLink) {
        deleteLink.addEventListener('click', function (e) {
            e.preventDefault();
            var itemId = this.getAttribute('data-id');
            Swal.fire({
                title: "Are you sure?",
                text: "You won't be able to revert this!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Yes, delete it!"
            }).then((result) => {
                if (result.isConfirmed) {
                    Swal.fire({
                        title: "Deleted!",
                        text: "Your file has been deleted.",
                        icon: "success",
                        confirmButtonColor: "#3085d6",
                        cancelButtonColor: "#d33",
                        confirmButtonText: "OK"
                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.href = "/WishList/RemoveProductFromWish?id=" + itemId;
                        }
                    }); // <-- Closing parenthesis was missing here
                }
            });
        });
    });
}