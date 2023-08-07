﻿namespace Templates.Blazor.EF.API;

#region << Using >>

using CRUD.Core;
using CRUD.CQRS;
using CRUD.DAL.Abstractions;
using CRUD.WebAPI;
using Microsoft.AspNetCore.Mvc;
using Templates.Blazor.EF.Shared;

#endregion

[Route("[controller]/[action]")]
public class ToDoListItemsController : DispatcherControllerBase
{
    #region Constructors

    public ToDoListItemsController(IDispatcher dispatcher) : base(dispatcher) { }

    #endregion

    [Route("~/" + ApiRoutes.ReadToDoListItems)]
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponseDto<ToDoListItemDto>), 200)]
    public async Task<IActionResult> Read([FromQuery(Name = ApiRoutes.Params.toDoListId)] int toDoListId,
                                          [FromQuery(Name = ApiRoutes.Params.page)]
                                          int? page,
                                          [FromQuery(Name = ApiRoutes.Params.pageSize)]
                                          int? pageSize,
                                          CancellationToken cancellationToken = new())
    {
        var response = await Dispatcher.QueryAsync(new ReadEntitiesQuery<ToDoListItemEntity, int, ToDoListItemDto>
                                                   {
                                                           PageSize = pageSize,
                                                           Page = page,
                                                           OrderSpecifications = new[]
                                                                                 {
                                                                                         new OrderById<ToDoListItemEntity, int>(false)
                                                                                 },
                                                           Specification = new ToDoListIdProp.FindByValue<ToDoListItemEntity>(toDoListId)
                                                   }, cancellationToken);

        return Ok(response);
    }

    [Route("~/" + ApiRoutes.CreateOrUpdateToDoListItem)]
    [HttpPost]
    [ProducesResponseType(200)]
    public async Task<IActionResult> CreateOrUpdate([FromBody] ToDoListItemDto dto,
                                                    CancellationToken cancellationToken = new())
    {
        var command = new CreateOrUpdateToDoListItemCommand { Dto = dto };
        await Dispatcher.PushAsync(command, cancellationToken);

        return Ok(command.Result);
    }

    [Route("~/" + ApiRoutes.DeleteToDoListItem)]
    [HttpDelete]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Delete([FromQuery(Name = ApiRoutes.Params.id)] int id,
                                            CancellationToken cancellationToken = new())
    {
        var command = new DeleteEntitiesCommand<ToDoListItemEntity, int>(new[] { id });
        await Dispatcher.PushAsync(command, cancellationToken);

        return Ok(command.Result);
    }
}