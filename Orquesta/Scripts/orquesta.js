
var id_pedido =0;

function crearTabla(input, input2, contenedor, hidden, input3) {

    var e = document.getElementById(input);
    var operador = document.getElementById(input3);

    if (e.options[e.selectedIndex].text !="" && document.getElementById(input2).value != "") {

        var closeSpan = document.createElement("span");
        closeSpan.setAttribute("class", "tag");
        closeSpan.setAttribute("name", $("#" + input).val());
        var texto1 = '';

        if (e.options[e.selectedIndex].text == 'ETHERNET') {
             texto1 = e.options[e.selectedIndex].text + ' - ' + document.getElementById(input2).value;
        } else {
            texto1 = e.options[e.selectedIndex].text + ' - ' + operador.options[operador.selectedIndex].text  + ' - ' + document.getElementById(input2).value;
        }

        
        closeSpan.innerHTML = texto1;

        var text_box_nombre = document.createElement("I");
        text_box_nombre.setAttribute("class", "closeTag");

        var columna_nombre = document.createElement("td");
        columna_nombre.style.height = "25px";

        var fila = document.createElement("tr");

        id_pedido = id_pedido + 1;

        fila.id = id_pedido;
      

        var tabla = document.getElementById(contenedor);
        tabla.style.marginLeft = "1%";
        tabla.style.marginTop = "1%";

        text_box_nombre.onclick = function () {

            $("#" + fila.id).remove();

            cargarHidden(contenedor, hidden, input, input2);

        };

        closeSpan.appendChild(text_box_nombre);
      //  p.appendChild(closeSpan);

        columna_nombre.appendChild(closeSpan);
        fila.appendChild(columna_nombre);
        tabla.appendChild(fila);

        cargarHidden(contenedor, hidden, input, input2);
        document.getElementById(input).value = "";
        document.getElementById(input2).value = "";

    }
}

function cargarHidden(tabla, campo, input, input2) {

    document.getElementById(campo).value = "";

    $("#" + tabla + " span").each(function (index, elem) {

        document.getElementById(campo).value = document.getElementById(campo).value + $(this).attr('name') + '-' + $(this).text() + '}';

    });


    if (document.getElementById(campo).value != ''){
         document.getElementById(campo).value = document.getElementById(campo).value.substring(0, document.getElementById(campo).value.length - 1) 
       }
    
}


function isNumberKey(e) {

    var evt = (e) ? e : window.event;
    var charCode = (evt.keyCode) ? evt.keyCode : evt.which;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}


$(document).ready(function () {

    $('#cant').on('paste', function (event) {
        if (event.originalEvent.clipboardData.getData('Text').match(/[^\d]/)) {
            event.preventDefault();
        }
    });


    $("#cant").on("change", function (event) {
        $value = $("#cant").val();
        if (isNaN($value)) {
            $("#cant").val('');
        }
    });


    $("#planx").on("change", function (e) {
        e.preventDefault();

      
        $.ajax({
            url: "http://localhost:54299/Pedido/getStockPlanes",
          //  url: '@Url.Action("getStockPlanes", "Pedido")',
            data: { busqueda: $('#plan').val() },
            type: "post",
            dataType: "json",
            success: function (result) {

                var div = $('div').find('#tbl');
                div.empty();
                var htmlText = "";

                $.each(result, function (items, item) {

                    htmlText += ("<table class='table'><tr><th>Plan</th><th>Cantidad</th>" +
                        "</tr><tr><td>" + item.Descripcion + "</td><td>" + item.Cantidad + "</td>" +
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



    $("#plan").on("change", function (event) {
        var e = document.getElementById('plan');
        filter = e.options[e.selectedIndex].text;

        if (filter == "ETHERNET"){
         
            $('#operador').hide();
        }else{
            $('#operador').show();
        }


    });

        





});