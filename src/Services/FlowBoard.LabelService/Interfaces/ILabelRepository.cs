using FlowBoard.LabelService.Entities;

namespace FlowBoard.LabelService.Interfaces;

public interface ILabelRepository
{
    Task<Label?>            GetByIdAsync(int labelId);
    Task<Label?>            GetByIdWithCardLabelsAsync(int labelId);
    Task<List<Label>>       GetByBoardIdAsync(int boardId);
    Task<bool>              ExistsAsync(int labelId);
    Task<Label>             CreateAsync(Label label);
    Task<Label>             UpdateAsync(Label label);
    Task                    DeleteAsync(int labelId);

    Task<CardLabel?>        GetCardLabelAsync(int cardId, int labelId);
    Task<List<CardLabel>>   GetCardLabelsAsync(int cardId);
    Task<bool>              IsAssignedAsync(int cardId, int labelId);
    Task<CardLabel>         AssignAsync(CardLabel cardLabel);
    Task                    UnassignAsync(int cardId, int labelId);
}