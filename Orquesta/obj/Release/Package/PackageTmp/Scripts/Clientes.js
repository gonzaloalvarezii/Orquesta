$(document).ready(function () {

    $("#FilterClick2").on("click", function (e) {
        e.preventDefault();

        $.ajax({
            url: "http://localhost:54299/Clientes/getClientes2",
           // url: '@Url.Action("getClientes2", "Clientes")',
            data: { busqueda: $('#busqueda').val() },
            type: "post",
            dataType: "json",
            success: function (result) {

                var div = $('div').find('#tbl');
                div.empty();
                var htmlText = "";

                $.each(result, function (items, item) {

                    htmlText += ("<table class='table'><tr><th>Nombre</th><th>RUT</th>" +
                        "</tr><tr><td>" + item.Nombre + "</td><td>" + item.RUT + "</td>" +
                        "<td>" + '@Html.Raw(HttpUtility.HtmlDecode(Html.ActionLink("Contratos", "Edit", new { /* id=item.PrimaryKey */ }).ToHtmlString()))' + " | " +
                        '@Html.Raw(HttpUtility.HtmlDecode(Html.ActionLink("Instalación", "Details", new { /* id=item.PrimaryKey */ }).ToHtmlString()))' + " | " +
                        '@Html.Raw(HttpUtility.HtmlDecode(Html.ActionLink("Sellos", "Delete", new { /* id=item.PrimaryKey */ }).ToHtmlString()))' + "</td>" +
                        "</tr>" +
                        "</table>");

                });

                div.append(htmlText);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    });
});