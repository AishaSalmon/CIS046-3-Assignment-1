const IMAGE_BASE_URL = import.meta.env.VITE_IMAGE_URL;

export const getImageUrl = (imagePath?: string | null): string => {
    if (!imagePath) return "";

    if (/^https?:\/\//i.test(imagePath)) {
        return imagePath;
    }

    return `${IMAGE_BASE_URL}/${imagePath.replace(/^\/?images\//, "")}`;
};