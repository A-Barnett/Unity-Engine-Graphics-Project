using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// // Created By: Alex Barnett
/// A custom render feature to create a fog effect in the Universal Render Pipeline.
/// </summary>
public class FogEffectRenderFeature : ScriptableRendererFeature
{
    /// <summary>
    /// The render pass for the fog effect.
    /// </summary>
    class FogEffectRenderPass : ScriptableRenderPass
    {
        private Material fogMaterial; // The material that performs the fog effect.
        private RenderTexture tempRenderTexture; // Temporary render texture for processing the effect.

        /// <summary>
        /// Initializes the fog effect render pass with the specified material and render pass event.
        /// </summary>
        public FogEffectRenderPass(Material material, RenderPassEvent passEvent)
        {
            this.fogMaterial = material;
            this.renderPassEvent = passEvent;
        }

        /// <summary>
        /// Configures the command buffer and temporary render texture before executing the pass.
        /// </summary>
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            tempRenderTexture = RenderTexture.GetTemporary(cameraTextureDescriptor);
        }

        /// <summary>
        /// Executes the render pass to apply the fog effect.
        /// </summary>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Require depth texture for fog calculations
            renderingData.cameraData.requiresDepthTexture = true;

            if (fogMaterial == null)
                return; // Do nothing if the fog material is not set

            CommandBuffer cmd = CommandBufferPool.Get("FogEffect");
            RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;

            // Copy the content of the current render target to the temporary texture
            cmd.Blit(source, tempRenderTexture);

            // Set the temporary render texture as the input for the fog shader
            fogMaterial.SetTexture("_MainTex", tempRenderTexture);

            // Apply the fog effect while copying back from the temporary render texture to the camera target
            cmd.Blit(tempRenderTexture, source, fogMaterial);

            // Execute the prepared commands
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            // Clean up
            RenderTexture.ReleaseTemporary(tempRenderTexture);
            CommandBufferPool.Release(cmd);
        }
    }

    public Material fogMaterial; // Public material property to assign the fog material.
    private FogEffectRenderPass fogPass; // Instance of the fog effect render pass.

    /// <summary>
    /// Called when the feature is created. Sets up the fog effect render pass.
    /// </summary>
    public override void Create()
    {
        fogPass = new FogEffectRenderPass(fogMaterial, RenderPassEvent.AfterRenderingTransparents);
    }

    /// <summary>
    /// Adds the fog effect render pass to the renderer if the fog material is set.
    /// </summary>
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (fogMaterial != null)
        {
            renderer.EnqueuePass(fogPass);
        }
    }
}
