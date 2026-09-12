using Discord;

namespace Kradis.Domain.Discord.Service;

public class DiscordEmbedService
{
    public Embed BuildWarningChannelAntiSpam()
    {
        var builder = new EmbedBuilder()
            .WithTitle(":warning: Warning! | Advertencia! :warning:")
            .WithDescription(
                "Do not post in this channel; it exists solely for banning users who spam or have compromised accounts. Posting here will result in a ban. If you post here by mistake, please contact an administrator.\n\n\nNo escriba en este canal. Este canal ha sido creado exclusivamente para banear a usuarios que hacen spam o cuyas cuentas han sido comprometidas. Escribir en este canal resultará en un baneo. Si escribe aquí por error, comuníquese con un administrador.")
            .WithFooter(
                text: "Kradis by GatoArtStudio",
                iconUrl: "https://cdn.discordapp.com/avatars/1544188681823457300/f5e0042c701a9737ba4bff1b95297d00.webp?size=1024")
            .WithColor(0xffff00);

        return builder.Build();
    }
}