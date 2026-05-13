using AutoMapper;
using FlowBoard.LabelService.DTOs;
using FlowBoard.LabelService.Entities;
using FlowBoard.LabelService.Interfaces;

namespace FlowBoard.LabelService.Services;

public class LabelServiceImpl : ILabelService
{
    private readonly ILabelRepository _repo;
    private readonly IMapper          _mapper;

    public LabelServiceImpl(ILabelRepository repo, IMapper mapper)
    {
        _repo   = repo;
        _mapper = mapper;
    }

    public async Task<LabelDto> CreateAsync(CreateLabelDto dto)
    {
        var label = new Label
        {
            BoardId = dto.BoardId,
            Name    = dto.Name,
            Color   = dto.Color
        };

        await _repo.CreateAsync(label);
        return _mapper.Map<LabelDto>(label);
    }

    public async Task<LabelDto> GetByIdAsync(int labelId)
    {
        var label = await _repo.GetByIdAsync(labelId)
            ?? throw new KeyNotFoundException("Label not found.");

        return _mapper.Map<LabelDto>(label);
    }

    public async Task<List<LabelDto>> GetByBoardIdAsync(int boardId)
    {
        var labels = await _repo.GetByBoardIdAsync(boardId);
        return _mapper.Map<List<LabelDto>>(labels);
    }

    public async Task<LabelDto> UpdateAsync(int labelId, UpdateLabelDto dto)
    {
        var label = await _repo.GetByIdAsync(labelId)
            ?? throw new KeyNotFoundException("Label not found.");

        if (dto.Name  != null) label.Name  = dto.Name;
        if (dto.Color != null) label.Color = dto.Color;

        await _repo.UpdateAsync(label);
        return _mapper.Map<LabelDto>(label);
    }

    public async Task DeleteAsync(int labelId)
    {
        if (!await _repo.ExistsAsync(labelId))
            throw new KeyNotFoundException("Label not found.");

        // Cascade delete removes all CardLabels too
        await _repo.DeleteAsync(labelId);
    }

    public async Task<CardLabelDto> AssignToCardAsync(
        int labelId, AssignLabelDto dto)
    {
        var label = await _repo.GetByIdAsync(labelId)
            ?? throw new KeyNotFoundException("Label not found.");

        if (await _repo.IsAssignedAsync(dto.CardId, labelId))
            throw new InvalidOperationException(
                "Label is already assigned to this card.");

        var cardLabel = new CardLabel
        {
            CardId  = dto.CardId,
            LabelId = labelId
        };

        await _repo.AssignAsync(cardLabel);

        // Return with label details included
        return new CardLabelDto
        {
            CardLabelId = cardLabel.CardLabelId,
            CardId      = cardLabel.CardId,
            LabelId     = labelId,
            LabelName   = label.Name,
            LabelColor  = label.Color,
            AssignedAt  = cardLabel.AssignedAt
        };
    }

    public async Task UnassignFromCardAsync(int labelId, int cardId)
    {
        if (!await _repo.IsAssignedAsync(cardId, labelId))
            throw new KeyNotFoundException(
                "Label is not assigned to this card.");

        await _repo.UnassignAsync(cardId, labelId);
    }

    public async Task<List<CardLabelDto>> GetCardLabelsAsync(int cardId)
    {
        var cardLabels = await _repo.GetCardLabelsAsync(cardId);

        return cardLabels.Select(cl => new CardLabelDto
        {
            CardLabelId = cl.CardLabelId,
            CardId      = cl.CardId,
            LabelId     = cl.LabelId,
            LabelName   = cl.Label.Name,
            LabelColor  = cl.Label.Color,
            AssignedAt  = cl.AssignedAt
        }).ToList();
    }
}