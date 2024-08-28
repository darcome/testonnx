public static class OnnxRuntimeGenAIKernelBuilderExtensions
{
    /// <summary>
    /// Add OnnxRuntimeGenAI Chat Completion services to the kernel builder.
    /// </summary>
    /// <param name="builder">The kernel builder.</param>
    /// <param name="modelPath">The generative AI ONNX model path.</param>
    /// <param name="serviceId">The optional service ID.</param>
    /// <returns>The updated kernel builder.</returns>
    public static IKernelBuilder AddOnnxRuntimeGenAIChatCompletion(
        this IKernelBuilder builder,
        string modelPath,
        string? serviceId = null)
    {
        builder.Services.AddKeyedSingleton<IChatCompletionService>(serviceId, (serviceProvider, _) =>
            new OnnxRuntimeGenAIChatCompletionService(
                modelPath: modelPath,
                loggerFactory: serviceProvider.GetService<ILoggerFactory>()));

        return builder;
    }
}