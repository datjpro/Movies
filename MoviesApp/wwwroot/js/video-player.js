// CCFilm Video Player - Modern HTML5 Video Player
class CCFilmVideoPlayer {
  constructor(container, options = {}) {
    this.container =
      typeof container === "string"
        ? document.querySelector(container)
        : container;
    this.options = {
      autoplay: false,
      controls: true,
      loop: false,
      muted: false,
      volume: 1,
      playbackRates: [0.25, 0.5, 0.75, 1, 1.25, 1.5, 2],
      qualities: [
        { label: "1080p", src: options.src || "" },
        { label: "720p", src: options.src720 || "" },
        { label: "480p", src: options.src480 || "" },
        { label: "360p", src: options.src360 || "" },
      ],
      subtitles: options.subtitles || [],
      ...options,
    };

    this.currentQuality = 0;
    this.currentRate = 1;
    this.isFullscreen = false;
    this.hideControlsTimeout = null;
    this.isDragging = false;

    this.init();
  }

  init() {
    this.createPlayer();
    this.bindEvents();
    this.loadVideo();
  }

  createPlayer() {
    this.container.innerHTML = `
            <div class="ccfilm-video-player" id="videoPlayer">
                <div class="video-container">
                    <video class="video-element" id="videoElement">
                        <source src="${
                          this.options.qualities[0].src
                        }" type="video/mp4">
                        Your browser does not support the video tag.
                    </video>
                    
                    <!-- Loading Spinner -->
                    <div class="video-loading" id="videoLoading">
                        <div class="loading-spinner"></div>
                    </div>
                    
                    <!-- Play Overlay -->
                    <div class="play-overlay" id="playOverlay">
                        <button class="play-overlay-btn" id="playOverlayBtn">
                            <i class="fas fa-play"></i>
                        </button>
                    </div>
                    
                    <!-- Subtitles -->
                    <div class="video-subtitles" id="videoSubtitles"></div>
                </div>
                
                <!-- Video Controls -->
                <div class="video-controls" id="videoControls">
                    <!-- Progress Bar -->
                    <div class="progress-container" id="progressContainer">
                        <div class="buffer-bar" id="bufferBar"></div>
                        <div class="progress-bar" id="progressBar">
                            <div class="progress-handle" id="progressHandle"></div>
                        </div>
                    </div>
                    
                    <!-- Control Buttons -->
                    <div class="controls-row">
                        <div class="controls-left">
                            <button class="control-btn play-pause" id="playPauseBtn">
                                <i class="fas fa-play"></i>
                            </button>
                            <button class="control-btn" id="prevBtn" title="Previous">
                                <i class="fas fa-step-backward"></i>
                            </button>
                            <button class="control-btn" id="nextBtn" title="Next">
                                <i class="fas fa-step-forward"></i>
                            </button>
                            <div class="time-display" id="timeDisplay">
                                <span id="currentTime">00:00</span> / <span id="duration">00:00</span>
                            </div>
                        </div>
                        
                        <div class="controls-center">
                            <button class="control-btn" id="rewindBtn" title="Rewind 10s">
                                <i class="fas fa-undo"></i>
                            </button>
                            <button class="control-btn" id="forwardBtn" title="Forward 10s">
                                <i class="fas fa-redo"></i>
                            </button>
                        </div>
                        
                        <div class="controls-right">
                            <div class="volume-control">
                                <button class="control-btn" id="muteBtn" title="Mute">
                                    <i class="fas fa-volume-up"></i>
                                </button>
                                <input type="range" class="volume-slider" id="volumeSlider" 
                                       min="0" max="1" step="0.1" value="1">
                            </div>
                            
                            <div class="settings-dropdown" id="settingsDropdown">
                                <button class="control-btn" id="settingsBtn" title="Settings">
                                    <i class="fas fa-cog"></i>
                                </button>
                                <div class="settings-menu" id="settingsMenu">
                                    <div class="settings-section">
                                        <span class="settings-label">Playback Speed</span>
                                        <div class="speed-buttons" id="speedButtons">
                                            ${this.options.playbackRates
                                              .map(
                                                (rate) =>
                                                  `<button class="speed-btn ${
                                                    rate === 1 ? "active" : ""
                                                  }" data-rate="${rate}">${rate}x</button>`
                                              )
                                              .join("")}
                                        </div>
                                    </div>
                                    
                                    <div class="settings-section">
                                        <span class="settings-label">Quality</span>
                                        <div class="quality-options" id="qualityOptions">
                                            ${this.options.qualities
                                              .filter((q) => q.src)
                                              .map(
                                                (quality, index) =>
                                                  `<div class="quality-option ${
                                                    index === 0 ? "active" : ""
                                                  }" data-quality="${index}">
                                                    ${quality.label}
                                                </div>`
                                              )
                                              .join("")}
                                        </div>
                                    </div>
                                    
                                    ${
                                      this.options.subtitles.length > 0
                                        ? `
                                    <div class="settings-section">
                                        <span class="settings-label">Subtitles</span>
                                        <div class="subtitle-options" id="subtitleOptions">
                                            <div class="settings-option active" data-subtitle="-1">Off</div>
                                            ${this.options.subtitles
                                              .map(
                                                (sub, index) =>
                                                  `<div class="settings-option" data-subtitle="${index}">${sub.label}</div>`
                                              )
                                              .join("")}
                                        </div>
                                    </div>
                                    `
                                        : ""
                                    }
                                </div>
                            </div>
                            
                            <button class="control-btn" id="pipBtn" title="Picture in Picture">
                                <i class="fas fa-external-link-alt"></i>
                            </button>
                            <button class="control-btn" id="fullscreenBtn" title="Fullscreen">
                                <i class="fas fa-expand"></i>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;

    // Get references to DOM elements
    this.player = this.container.querySelector("#videoPlayer");
    this.video = this.container.querySelector("#videoElement");
    this.controls = this.container.querySelector("#videoControls");
    this.playPauseBtn = this.container.querySelector("#playPauseBtn");
    this.playOverlay = this.container.querySelector("#playOverlay");
    this.playOverlayBtn = this.container.querySelector("#playOverlayBtn");
    this.progressContainer = this.container.querySelector("#progressContainer");
    this.progressBar = this.container.querySelector("#progressBar");
    this.progressHandle = this.container.querySelector("#progressHandle");
    this.bufferBar = this.container.querySelector("#bufferBar");
    this.currentTimeEl = this.container.querySelector("#currentTime");
    this.durationEl = this.container.querySelector("#duration");
    this.volumeSlider = this.container.querySelector("#volumeSlider");
    this.muteBtn = this.container.querySelector("#muteBtn");
    this.settingsBtn = this.container.querySelector("#settingsBtn");
    this.settingsMenu = this.container.querySelector("#settingsMenu");
    this.settingsDropdown = this.container.querySelector("#settingsDropdown");
    this.fullscreenBtn = this.container.querySelector("#fullscreenBtn");
    this.pipBtn = this.container.querySelector("#pipBtn");
    this.rewindBtn = this.container.querySelector("#rewindBtn");
    this.forwardBtn = this.container.querySelector("#forwardBtn");
    this.loading = this.container.querySelector("#videoLoading");
    this.subtitlesEl = this.container.querySelector("#videoSubtitles");
  }

  bindEvents() {
    // Video events
    this.video.addEventListener("loadstart", () => this.showLoading());
    this.video.addEventListener("canplay", () => this.hideLoading());
    this.video.addEventListener("loadedmetadata", () => this.updateDuration());
    this.video.addEventListener("timeupdate", () => this.updateProgress());
    this.video.addEventListener("progress", () => this.updateBuffer());
    this.video.addEventListener("play", () => this.onPlay());
    this.video.addEventListener("pause", () => this.onPause());
    this.video.addEventListener("ended", () => this.onEnded());
    this.video.addEventListener("waiting", () => this.showLoading());
    this.video.addEventListener("playing", () => this.hideLoading());

    // Control events
    this.playPauseBtn.addEventListener("click", () => this.togglePlayPause());
    this.playOverlayBtn.addEventListener("click", () => this.togglePlayPause());
    this.video.addEventListener("click", () => this.togglePlayPause());

    // Progress bar events
    this.progressContainer.addEventListener("click", (e) => this.seekTo(e));
    this.progressContainer.addEventListener("mousedown", (e) =>
      this.startDrag(e)
    );
    document.addEventListener("mousemove", (e) => this.onDrag(e));
    document.addEventListener("mouseup", () => this.endDrag());

    // Volume events
    this.volumeSlider.addEventListener("input", (e) =>
      this.setVolume(e.target.value)
    );
    this.muteBtn.addEventListener("click", () => this.toggleMute());

    // Settings events
    this.settingsBtn.addEventListener("click", (e) => {
      e.stopPropagation();
      this.toggleSettings();
    });
    document.addEventListener("click", () => this.closeSettings());

    // Speed control
    this.container.querySelectorAll(".speed-btn").forEach((btn) => {
      btn.addEventListener("click", (e) =>
        this.setPlaybackRate(parseFloat(e.target.dataset.rate))
      );
    });

    // Quality control
    this.container.querySelectorAll(".quality-option").forEach((option) => {
      option.addEventListener("click", (e) =>
        this.setQuality(parseInt(e.target.dataset.quality))
      );
    });

    // Subtitle control
    this.container.querySelectorAll("[data-subtitle]").forEach((option) => {
      option.addEventListener("click", (e) =>
        this.setSubtitle(parseInt(e.target.dataset.subtitle))
      );
    });

    // Fullscreen events
    this.fullscreenBtn.addEventListener("click", () => this.toggleFullscreen());
    document.addEventListener("fullscreenchange", () =>
      this.onFullscreenChange()
    );

    // Picture in Picture
    this.pipBtn.addEventListener("click", () => this.togglePiP());

    // Navigation buttons
    this.rewindBtn.addEventListener("click", () => this.rewind());
    this.forwardBtn.addEventListener("click", () => this.forward());

    // Keyboard shortcuts
    document.addEventListener("keydown", (e) => this.handleKeyboard(e));

    // Mouse movement for controls
    this.player.addEventListener("mousemove", () => this.showControls());
    this.player.addEventListener("mouseleave", () => this.autoHideControls());
  }

  loadVideo() {
    if (this.options.qualities[0].src) {
      this.video.src = this.options.qualities[0].src;
      this.video.volume = this.options.volume;
      this.video.muted = this.options.muted;

      if (this.options.autoplay) {
        this.video.play();
      }
    }
  }

  // Playback controls
  togglePlayPause() {
    if (this.video.paused) {
      this.play();
    } else {
      this.pause();
    }
  }

  play() {
    this.video.play();
  }

  pause() {
    this.video.pause();
  }

  onPlay() {
    this.playPauseBtn.innerHTML = '<i class="fas fa-pause"></i>';
    this.player.classList.remove("paused");
    this.autoHideControls();
  }

  onPause() {
    this.playPauseBtn.innerHTML = '<i class="fas fa-play"></i>';
    this.player.classList.add("paused");
    this.showControls();
  }

  onEnded() {
    this.player.classList.add("paused");
    this.showControls();
  }

  // Progress and seeking
  updateProgress() {
    if (!this.isDragging) {
      const percent = (this.video.currentTime / this.video.duration) * 100;
      this.progressBar.style.width = percent + "%";
      this.updateTimeDisplay();
    }
  }

  updateBuffer() {
    if (this.video.buffered.length > 0) {
      const bufferedEnd = this.video.buffered.end(
        this.video.buffered.length - 1
      );
      const duration = this.video.duration;
      const bufferedPercent = (bufferedEnd / duration) * 100;
      this.bufferBar.style.width = bufferedPercent + "%";
    }
  }

  seekTo(e) {
    const rect = this.progressContainer.getBoundingClientRect();
    const percent = (e.clientX - rect.left) / rect.width;
    const time = percent * this.video.duration;
    this.video.currentTime = time;
  }

  startDrag(e) {
    this.isDragging = true;
    this.seekTo(e);
  }

  onDrag(e) {
    if (this.isDragging) {
      this.seekTo(e);
    }
  }

  endDrag() {
    this.isDragging = false;
  }

  // Time display
  updateDuration() {
    this.durationEl.textContent = this.formatTime(this.video.duration);
  }

  updateTimeDisplay() {
    this.currentTimeEl.textContent = this.formatTime(this.video.currentTime);
  }

  formatTime(seconds) {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const secs = Math.floor(seconds % 60);

    if (hours > 0) {
      return `${hours.toString().padStart(2, "0")}:${minutes
        .toString()
        .padStart(2, "0")}:${secs.toString().padStart(2, "0")}`;
    }
    return `${minutes.toString().padStart(2, "0")}:${secs
      .toString()
      .padStart(2, "0")}`;
  }

  // Volume controls
  setVolume(volume) {
    this.video.volume = volume;
    this.updateVolumeIcon();
  }

  toggleMute() {
    this.video.muted = !this.video.muted;
    this.updateVolumeIcon();
  }

  updateVolumeIcon() {
    const icon = this.muteBtn.querySelector("i");
    if (this.video.muted || this.video.volume === 0) {
      icon.className = "fas fa-volume-mute";
    } else if (this.video.volume < 0.5) {
      icon.className = "fas fa-volume-down";
    } else {
      icon.className = "fas fa-volume-up";
    }
  }

  // Settings
  toggleSettings() {
    this.settingsDropdown.classList.toggle("active");
  }

  closeSettings() {
    this.settingsDropdown.classList.remove("active");
  }

  setPlaybackRate(rate) {
    this.video.playbackRate = rate;
    this.currentRate = rate;

    // Update active button
    this.container.querySelectorAll(".speed-btn").forEach((btn) => {
      btn.classList.toggle("active", parseFloat(btn.dataset.rate) === rate);
    });
  }

  setQuality(qualityIndex) {
    if (
      qualityIndex < this.options.qualities.length &&
      this.options.qualities[qualityIndex].src
    ) {
      const currentTime = this.video.currentTime;
      const wasPlaying = !this.video.paused;

      this.currentQuality = qualityIndex;
      this.video.src = this.options.qualities[qualityIndex].src;

      this.video.addEventListener(
        "loadedmetadata",
        () => {
          this.video.currentTime = currentTime;
          if (wasPlaying) {
            this.video.play();
          }
        },
        { once: true }
      );

      // Update active option
      this.container
        .querySelectorAll(".quality-option")
        .forEach((option, index) => {
          option.classList.toggle("active", index === qualityIndex);
        });
    }
  }

  setSubtitle(subtitleIndex) {
    // Update active option
    this.container
      .querySelectorAll("[data-subtitle]")
      .forEach((option, index) => {
        option.classList.toggle(
          "active",
          parseInt(option.dataset.subtitle) === subtitleIndex
        );
      });

    // Hide/show subtitles (would need actual subtitle implementation)
    if (subtitleIndex === -1) {
      this.subtitlesEl.style.display = "none";
    } else {
      this.subtitlesEl.style.display = "block";
      // Load subtitle file logic would go here
    }
  }

  // Fullscreen
  toggleFullscreen() {
    if (!this.isFullscreen) {
      this.enterFullscreen();
    } else {
      this.exitFullscreen();
    }
  }

  enterFullscreen() {
    if (this.player.requestFullscreen) {
      this.player.requestFullscreen();
    } else if (this.player.webkitRequestFullscreen) {
      this.player.webkitRequestFullscreen();
    } else if (this.player.msRequestFullscreen) {
      this.player.msRequestFullscreen();
    }
  }

  exitFullscreen() {
    if (document.exitFullscreen) {
      document.exitFullscreen();
    } else if (document.webkitExitFullscreen) {
      document.webkitExitFullscreen();
    } else if (document.msExitFullscreen) {
      document.msExitFullscreen();
    }
  }

  onFullscreenChange() {
    this.isFullscreen = !!document.fullscreenElement;
    this.player.classList.toggle("fullscreen", this.isFullscreen);

    const icon = this.fullscreenBtn.querySelector("i");
    icon.className = this.isFullscreen ? "fas fa-compress" : "fas fa-expand";
  }

  // Picture in Picture
  togglePiP() {
    if (document.pictureInPictureElement) {
      document.exitPictureInPicture();
    } else if (document.pictureInPictureEnabled) {
      this.video.requestPictureInPicture();
    }
  }

  // Navigation
  rewind() {
    this.video.currentTime = Math.max(0, this.video.currentTime - 10);
  }

  forward() {
    this.video.currentTime = Math.min(
      this.video.duration,
      this.video.currentTime + 10
    );
  }

  // Controls visibility
  showControls() {
    this.player.classList.add("show-controls");
    this.clearHideTimeout();
  }

  hideControls() {
    this.player.classList.remove("show-controls");
  }

  autoHideControls() {
    this.clearHideTimeout();
    this.hideControlsTimeout = setTimeout(() => {
      if (!this.video.paused) {
        this.hideControls();
      }
    }, 3000);
  }

  clearHideTimeout() {
    if (this.hideControlsTimeout) {
      clearTimeout(this.hideControlsTimeout);
      this.hideControlsTimeout = null;
    }
  }

  // Loading states
  showLoading() {
    this.loading.style.display = "block";
  }

  hideLoading() {
    this.loading.style.display = "none";
  }

  // Keyboard shortcuts
  handleKeyboard(e) {
    if (e.target.tagName === "INPUT") return;

    switch (e.code) {
      case "Space":
        e.preventDefault();
        this.togglePlayPause();
        break;
      case "ArrowLeft":
        e.preventDefault();
        this.rewind();
        break;
      case "ArrowRight":
        e.preventDefault();
        this.forward();
        break;
      case "ArrowUp":
        e.preventDefault();
        this.setVolume(Math.min(1, this.video.volume + 0.1));
        this.volumeSlider.value = this.video.volume;
        break;
      case "ArrowDown":
        e.preventDefault();
        this.setVolume(Math.max(0, this.video.volume - 0.1));
        this.volumeSlider.value = this.video.volume;
        break;
      case "KeyM":
        e.preventDefault();
        this.toggleMute();
        break;
      case "KeyF":
        e.preventDefault();
        this.toggleFullscreen();
        break;
    }
  }

  // Public API
  destroy() {
    this.clearHideTimeout();
    this.container.innerHTML = "";
  }

  getCurrentTime() {
    return this.video.currentTime;
  }

  getDuration() {
    return this.video.duration;
  }

  setCurrentTime(time) {
    this.video.currentTime = time;
  }

  getVolume() {
    return this.video.volume;
  }

  isPaused() {
    return this.video.paused;
  }
}

// Auto-initialize players
document.addEventListener("DOMContentLoaded", function () {
  // Initialize all video players on the page
  document.querySelectorAll("[data-video-player]").forEach((container) => {
    const options = JSON.parse(container.dataset.videoPlayer || "{}");
    new CCFilmVideoPlayer(container, options);
  });
});

// Export for module use
if (typeof module !== "undefined" && module.exports) {
  module.exports = CCFilmVideoPlayer;
}
