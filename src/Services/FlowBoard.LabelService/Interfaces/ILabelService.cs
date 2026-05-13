using FlowBoard.LabelService.DTOs;

namespace FlowBoard.LabelService.Interfaces;

public interface ILabelService
{
    Task<LabelDto>          CreateAsync(CreateLabelDto dto);
    Task<LabelDto>          GetByIdAsync(int labelId);
    Task<List<LabelDto>>    GetByBoardIdAsync(int boardId);
    Task<LabelDto>          UpdateAsync(int labelId, UpdateLabelDto dto);
    Task                    DeleteAsync(int labelId);

    Task<CardLabelDto>      AssignToCardAsync(int labelId, AssignLabelDto dto);
    Task                    UnassignFromCardAsync(int labelId, int cardId);
    Task<List<CardLabelDto>> GetCardLabelsAsync(int cardId);
}