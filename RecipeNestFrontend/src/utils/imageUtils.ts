const IMAGE_BASE_URL = import.meta.env.VITE_IMAGE_URL;

export const getImageUrl = (imagePath?: string | null): string => {
    if (!imagePath) return "";

    // Redirect locally stored images to Cloudflare R2
    if (imagePath.includes("/images/")) {
        const fileName = imagePath.split("/images/").pop();
        return `${IMAGE_BASE_URL}/${fileName}`;
    }

    // Preserve external image URLs
    if (imagePath.startsWith("http")) {
        return imagePath;
    }

    return `${IMAGE_BASE_URL}/${imagePath}`;
};