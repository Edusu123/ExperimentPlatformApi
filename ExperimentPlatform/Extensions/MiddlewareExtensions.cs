namespace ExperimentPlatform.Extensions
{
    public static class MiddlewareExtensions
    {
        public static WebApplication UseMiddlewareDefaults(
        this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseExceptionHandler("/error");

            app.UseAuthorization();

            return app;
        }

    }
}
