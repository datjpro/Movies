// Secure Video Player JavaScript
class SecureVideoPlayer extends CCFilmVideoPlayer {
  constructor(container, options = {}) {
    super(container, options);
    this.securityOptions = {
      tokenRefreshThreshold: 300, // 5 minutes before expiry
      maxRetries: 3,
      ...options.security,
    };
    this.currentToken = null;
    this.tokenExpiresAt = null;
    this.tokenRefreshTimer = null;
  }

  async init() {
    // Initialize with secure video loading
    await this.loadSecureVideo();
    super.init();
  }

  async loadSecureVideo() {
    try {
      if (!this.options.videoId) {
        throw new Error("VideoId is required for secure playback");
      }

      // Get secure video URL with token
      const response = await fetch(
        `/api/SecureVideo/video-url/${this.options.videoId}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${this.getAuthToken()}`,
            "Content-Type": "application/json",
          },
        }
      );

      if (!response.ok) {
        throw new Error(`Failed to get secure video URL: ${response.status}`);
      }

      const result = await response.json();

      if (result.success) {
        this.currentToken = this.extractTokenFromUrl(result.secureUrl);
        this.tokenExpiresAt = new Date(result.expiresAt);

        // Update video sources với secure URLs
        this.updateVideoSources(result.secureUrl);

        // Setup token refresh
        this.setupTokenRefresh();

        console.log("Secure video loaded successfully");
      } else {
        throw new Error("Failed to get secure video URL");
      }
    } catch (error) {
      console.error("Error loading secure video:", error);
      this.showError("Không thể tải video an toàn. Vui lòng thử lại.");
    }
  }

  updateVideoSources(secureUrl) {
    const videoElement = this.container.querySelector(".video-element");
    if (videoElement) {
      // Clear existing sources
      videoElement.innerHTML = "";

      // Add secure source
      const source = document.createElement("source");
      source.src = secureUrl;
      source.type = "video/mp4";
      videoElement.appendChild(source);

      // Load the new source
      videoElement.load();
    }
  }

  setupTokenRefresh() {
    if (this.tokenRefreshTimer) {
      clearTimeout(this.tokenRefreshTimer);
    }

    if (!this.tokenExpiresAt) return;

    const now = new Date();
    const timeUntilExpiry = this.tokenExpiresAt.getTime() - now.getTime();
    const refreshTime =
      timeUntilExpiry - this.securityOptions.tokenRefreshThreshold * 1000;

    if (refreshTime > 0) {
      this.tokenRefreshTimer = setTimeout(() => {
        this.refreshToken();
      }, refreshTime);
    }
  }

  async refreshToken() {
    try {
      console.log("Refreshing video token...");

      const response = await fetch(
        `/api/SecureVideo/video-url/${this.options.videoId}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${this.getAuthToken()}`,
            "Content-Type": "application/json",
          },
        }
      );

      if (response.ok) {
        const result = await response.json();
        if (result.success) {
          this.currentToken = this.extractTokenFromUrl(result.secureUrl);
          this.tokenExpiresAt = new Date(result.expiresAt);

          // Update video source without interrupting playback
          this.updateVideoSourceSilently(result.secureUrl);

          // Setup next refresh
          this.setupTokenRefresh();

          console.log("Token refreshed successfully");
        }
      }
    } catch (error) {
      console.error("Error refreshing token:", error);
      // Retry logic
      this.retryTokenRefresh();
    }
  }

  async retryTokenRefresh(attempt = 1) {
    if (attempt <= this.securityOptions.maxRetries) {
      console.log(
        `Retrying token refresh (attempt ${attempt}/${this.securityOptions.maxRetries})`
      );
      setTimeout(() => {
        this.refreshToken();
      }, attempt * 2000); // Exponential backoff
    } else {
      console.error("Max retry attempts reached for token refresh");
      this.showError("Phiên xem video đã hết hạn. Vui lòng tải lại trang.");
    }
  }

  updateVideoSourceSilently(secureUrl) {
    // Update source without stopping current playback
    const videoElement = this.container.querySelector(".video-element");
    if (videoElement && !videoElement.paused) {
      // Store current playback state
      const currentTime = videoElement.currentTime;
      const isPlaying = !videoElement.paused;

      // Update source
      this.updateVideoSources(secureUrl);

      // Restore playback state
      videoElement.addEventListener(
        "loadedmetadata",
        () => {
          videoElement.currentTime = currentTime;
          if (isPlaying) {
            videoElement.play();
          }
        },
        { once: true }
      );
    } else {
      this.updateVideoSources(secureUrl);
    }
  }

  extractTokenFromUrl(url) {
    try {
      const urlObj = new URL(url);
      return urlObj.searchParams.get("token");
    } catch {
      return null;
    }
  }

  getAuthToken() {
    // Get authentication token from localStorage, cookie, or other secure storage
    return (
      localStorage.getItem("auth_token") ||
      document.querySelector('meta[name="auth-token"]')?.content ||
      ""
    );
  }

  showError(message) {
    const errorDiv = document.createElement("div");
    errorDiv.className = "video-error-message";
    errorDiv.innerHTML = `
            <div class="alert alert-danger">
                <i class="fas fa-exclamation-triangle"></i>
                ${message}
            </div>
        `;

    const videoContainer = this.container.querySelector(".video-container");
    if (videoContainer) {
      videoContainer.appendChild(errorDiv);
    }
  }

  // Override destroy để cleanup timers
  destroy() {
    if (this.tokenRefreshTimer) {
      clearTimeout(this.tokenRefreshTimer);
    }
    super.destroy && super.destroy();
  }

  // Anti-debugging measures
  static initAntiDebugging() {
    // Disable right-click context menu
    document.addEventListener("contextmenu", (e) => {
      if (e.target.closest(".ccfilm-video-player")) {
        e.preventDefault();
      }
    });

    // Disable F12, Ctrl+Shift+I, Ctrl+U
    document.addEventListener("keydown", (e) => {
      if (e.target.closest(".ccfilm-video-player")) {
        if (
          e.key === "F12" ||
          (e.ctrlKey && e.shiftKey && e.key === "I") ||
          (e.ctrlKey && e.key === "u")
        ) {
          e.preventDefault();
          console.clear();
          console.log(
            "%cStop!",
            "color: red; font-size: 50px; font-weight: bold;"
          );
          console.log(
            '%cĐây là tính năng dành cho nhà phát triển. Nếu ai đó bảo bạn copy/paste thứ gì đó vào đây để "hack" hoặc "bypass" tính năng, đó là lừa đảo.',
            "color: red; font-size: 16px;"
          );
        }
      }
    });

    // Clear console periodically
    setInterval(() => {
      if (document.querySelector(".ccfilm-video-player")) {
        console.clear();
      }
    }, 1000);

    // Detect DevTools
    let devtools = { open: false, orientation: null };
    const threshold = 160;

    setInterval(() => {
      if (
        window.outerHeight - window.innerHeight > threshold ||
        window.outerWidth - window.innerWidth > threshold
      ) {
        if (!devtools.open) {
          devtools.open = true;
          console.log("%cDevTools detected!", "color: red; font-size: 20px;");
          // Optional: Pause all videos or show warning
          document.querySelectorAll(".video-element").forEach((video) => {
            if (!video.paused) {
              video.pause();
            }
          });
        }
      } else {
        devtools.open = false;
      }
    }, 500);
  }
}

// Initialize anti-debugging khi trang load
document.addEventListener("DOMContentLoaded", () => {
  SecureVideoPlayer.initAntiDebugging();
});

// Export cho global use
window.SecureVideoPlayer = SecureVideoPlayer;
