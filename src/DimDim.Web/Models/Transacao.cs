using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DimDim.Web.Models;

public enum TipoTransacao
{
    PIX,
    BOLETO,
    CARTAO
}

public enum StatusTransacao
{
    PENDENTE,
    PAGO,
    CANCELADO
}

public class Transacao
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Selecione o cliente.")]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    [ForeignKey(nameof(ClienteId))]
    public Cliente? Cliente { get; set; }

    [Required(ErrorMessage = "Informe a descrição.")]
    [StringLength(200)]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 1_000_000_000, ErrorMessage = "O valor deve ser maior que zero.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "Selecione o tipo.")]
    public TipoTransacao Tipo { get; set; }

    public StatusTransacao Status { get; set; } = StatusTransacao.PENDENTE;

    [Display(Name = "Data da transação")]
    public DateTime DataTransacao { get; set; } = DateTime.UtcNow;
}
