using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Created by: Alex Barnett
/// Custom lens flare feature for the Scriptable Render Pipeline.
/// </summary>
public class CustomLensFlareFeature : ScriptableRendererFeature
{
    /// <summary>
    /// The render pass for the custom lens flare effect.
    /// </summary>
    class CustomLensFlarePass : ScriptableRenderPass
    {
        private Material lensFlareMat; // The material that performs the lens flare effect.
        private RenderTargetIdentifier currentTarget; // The current render target identifier.
        private RenderTexture tempRenderTexture; // Temporary render texture for the effect.

        /// <summary>
        /// Initializes the lens flare pass with the specified event and material.
        /// </summary>
        public CustomLensFlarePass(RenderPassEvent passEvent, Material material)
        {
            this.renderPassEvent = passEvent;
            this.lensFlareMat = material;
        }

        /// <summary>
        /// Configures the command buffer before executing the pass.
        /// </summary>
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            tempRenderTexture = RenderTexture.GetTemporary(cameraTextureDescriptor);
        }

        /// <summary>
        /// Executes the render pass to apply the lens flare effect.
        /// </summary>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("CustomLensFlare");
            RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;
            cmd.Blit(source, tempRenderTexture);
            lensFlareMat.SetTexture("_MainTex", tempRenderTexture);
            cmd.Blit(tempRenderTexture, source, lensFlareMat);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            RenderTexture.ReleaseTemporary(tempRenderTexture);
            CommandBufferPool.Release(cmd);
        }
    }

    private CustomLensFlarePass flarePass; // The custom flare pass instance.
    public Material lensFlareMat; // Public material property to assign the lens flare material.

    /// <summary>
    /// Called when the feature is created. Sets up the lens flare pass.
    /// </summary>
    public override void Create()
    {
        flarePass = new CustomLensFlarePass(RenderPassEvent.AfterRenderingTransparents, lensFlareMat);
    }

    /// <summary>
    /// Adds the custom lens flare render pass to the renderer.
    /// </summary>
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(flarePass);
    }
}
