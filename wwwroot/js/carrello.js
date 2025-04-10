function aggiungiAlCarrello(prodottoId, quantita = 1) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    
    $.ajax({
        url: '/Carrello/AggiungiAlCarrello',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ 
            ProdottoId: prodottoId, 
            Quantita: quantita 
        }),
        headers: {
            'RequestVerificationToken': token
        },
        success: function(response) {
            if (response.success) {
                aggiornaContatoreCarrello();
                mostraNotifica('success', response.message);
            } else {
                mostraNotifica('error', response.message);
            }
        },
        error: function(xhr) {
            mostraNotifica('error', xhr.responseJSON?.message || 'Errore durante l\'aggiunta al carrello');
        }
    });
}

function rimuoviDalCarrello(prodottoId) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    
    $.ajax({
        url: '/Carrello/RimuoviDalCarrello',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ 
            ProdottoId: prodottoId 
        }),
        headers: {
            'RequestVerificationToken': token
        },
        success: function(response) {
            if (response.success) {
                aggiornaContatoreCarrello();
                mostraNotifica('success', response.message);
                $(`#carrello-item-${prodottoId}`).remove();
                if ($('.carrello-item').length === 0) {
                    location.reload();
                }
            } else {
                mostraNotifica('error', response.message);
            }
        },
        error: function(xhr) {
            mostraNotifica('error', xhr.responseJSON?.message || 'Errore durante la rimozione dal carrello');
        }
    });
}

function aggiornaQuantita(prodottoId, quantita) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    
    $.ajax({
        url: '/Carrello/AggiornaQuantita',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ 
            ProdottoId: prodottoId, 
            Quantita: quantita 
        }),
        headers: {
            'RequestVerificationToken': token
        },
        success: function(response) {
            if (response.success) {
                aggiornaContatoreCarrello();
                mostraNotifica('success', response.message);
            } else {
                mostraNotifica('error', response.message);
            }
        },
        error: function(xhr) {
            mostraNotifica('error', xhr.responseJSON?.message || 'Errore durante l\'aggiornamento');
        }
    });
}

function aggiornaContatoreCarrello() {
    $.get('/Carrello/GetContatore', function(count) {
        $('#carrello-contatore').text(count);
    });
}

function mostraNotifica(tipo, messaggio) {
    const toast = $(`
        <div class="toast align-items-center text-white bg-${tipo === 'success' ? 'success' : 'danger'} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    ${messaggio}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `);
    
    $('#toast-container').append(toast);
    const bsToast = new bootstrap.Toast(toast);
    bsToast.show();
    
    toast.on('hidden.bs.toast', function() {
        toast.remove();
    });
} 