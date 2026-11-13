using System.ComponentModel.DataAnnotations;

namespace ViceNews.API.DTOs;

/// <summary>
/// Editable vehicle fields shared by create and update requests.
/// Value types are nullable so that missing JSON properties fail validation instead of defaulting to 0.
/// </summary>
public abstract record VehicleRequestDtoBase
{
    [Required(ErrorMessage = "Informe o nome do veículo.")]
    [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Selecione o fabricante.")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um fabricante válido.")]
    public int? ManufacturerId { get; init; }

    [Required(ErrorMessage = "Selecione a categoria principal.")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria válida.")]
    public int? CategoryId { get; init; }

    [Required(ErrorMessage = "Informe o preço em GTA$.")]
    [Range(0, long.MaxValue, ErrorMessage = "O preço deve ser maior ou igual a zero.")]
    public long? PriceGta { get; init; }

    [Required(ErrorMessage = "Informe a descrição do veículo.")]
    [StringLength(300, ErrorMessage = "A descrição deve ter no máximo 300 caracteres.")]
    public string Description { get; init; } = string.Empty;

    [Required(ErrorMessage = "Informe a velocidade máxima.")]
    [Range(0, 100, ErrorMessage = "A velocidade máxima deve estar entre 0 e 100.")]
    public short? MaxSpeed { get; init; }

    [Required(ErrorMessage = "Informe a aceleração.")]
    [Range(0, 100, ErrorMessage = "A aceleração deve estar entre 0 e 100.")]
    public short? Acceleration { get; init; }

    [Required(ErrorMessage = "Informe a frenagem.")]
    [Range(0, 100, ErrorMessage = "A frenagem deve estar entre 0 e 100.")]
    public short? Braking { get; init; }

    [Required(ErrorMessage = "Informe o manuseio.")]
    [Range(0, 100, ErrorMessage = "O manuseio deve estar entre 0 e 100.")]
    public short? Handling { get; init; }

    public string? ImageUrl { get; init; }

    /// <summary>Tag names; the API finds or creates the matching VehicleTag records.</summary>
    public IReadOnlyList<string?>? Tags { get; init; }
}
