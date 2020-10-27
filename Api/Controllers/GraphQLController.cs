using System.Threading.Tasks;
using Api.Models;
using GraphQL.Types;
using GraphQL;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Api.Controllers
{
    [Route("graphql")]
    public class GraphQLController : ControllerBase
    {
        private readonly IDocumentExecuter documentExecuter;
        private readonly ISchema schema;

        public GraphQLController(IDocumentExecuter documentExecuter, ISchema schema)
        {
            this.documentExecuter = documentExecuter;
            this.schema = schema;
        }

        [HttpPost]
        public async Task<IActionResult> Query([FromBody] GraphQLQuery query)
        {
            var inputs = query.Variables?
                .ToObject<Dictionary<string, object>>().ToInputs();

            var result = await this.documentExecuter
                .ExecuteAsync(configure =>
            {
                configure.Schema = this.schema;
                configure.Query = query.Query;
                configure.OperationName = query.OperationName;
                configure.Inputs = inputs;
            }).ConfigureAwait(false);

            if (result.Errors?.Count > 0)
            {
                var errors = new List<object>();
                foreach (var err in result.Errors)
                {
                    errors.Add(new { err.Code, err.Message, err.Locations });
                }

                return BadRequest(new { errors });
            }

            return Ok(result.Data);
        }
    }
}
