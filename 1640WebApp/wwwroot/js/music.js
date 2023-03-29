

new Vue({
    el: "#app",
    data() {
        return {
            audio: null,
            circleLeft: null,
            barWidth: null,
            duration: null,
            currentTime: null,
            isTimerPlaying: false,
            tracks: [
                {
                    name: "Chờ Đợi Có Đáng Sợ (Lofi Ver)",
                    artist: "Andiez x Freak D",
                    cover: "/img/1.jpg",
                    source: "/mp3/1.mp3",
                    url: "https://www.youtube.com/watch?v=ko6KQq04qIc&ab_channel=FreakDMusic",
                    favorited: false
                },
                {
                    name: "Chắc Vì Mình Chưa Tốt ( Lofi Ver)",
                    artist: "Thanh Hưng x Vux",
                    cover: "/img/2.jpg",
                    source: "/mp3/2.mp3",
                    url: "https://www.youtube.com/watch?v=hmPsclre6BQ&ab_channel=VUXMusic",
                    favorited: true
                },
                {
                    name: "Cảm Ơn Vì Tất Cả (Lofi Ver)",
                    artist: "Anh Quân Idol x Haky",
                    cover: "/img/3.jpg",
                    source: "/mp3/3.mp3",
                    url: "https://www.youtube.com/watch?v=5V6bHm1pklA&ab_channel=FreakDMusic",
                    favorited: false
                },
                {
                    name: "Bước Qua Mùa Cô Đơn (Lofi Ver)",
                    artist: "Vũ x Hyarways x Enderlazer",
                    cover: "/img/4.jpg",
                    source: "/mp3/4.mp3",
                    url: "https://www.youtube.com/watch?v=PbuNHgWzFck&ab_channel=Coconyeuanh%3F",
                    favorited: true
                },
                {
                    name: "BÔNG HOA CHẲNG THUỘC VỀ TA",
                    artist: "NHƯ VIỆT",
                    cover: "/img/5.jpg",
                    source: "/mp3/5.mp3",
                    url: "https://www.youtube.com/watch?v=nV7_ZujXqKc&ab_channel=DeusTi%E1%BA%BFn%C4%90%E1%BA%A1t",
                    favorited: false
                },
                {
                    name: "Anh Yêu Vội Thế (Lofi Ver.)",
                    artist: "LaLa Trần x Freak D",
                    cover: "/img/6.jpg",
                    source: "/mp3/6.mp3",
                    url: "https://www.youtube.com/watch?v=OMTnQdoWvLM&ab_channel=FreakDMusic",
                    favorited: true
                },
                {
                    name: "Ánh Sao Và Bầu Trời -[Official Audio]",
                    artist: "T.R.I x Cá",
                    cover: "/img/7.jpg",
                    source: "/mp3/7.mp3",
                    url: "https://www.youtube.com/watch?v=9vaLkYElidg&ab_channel=T.R.I",
                    favorited: false
                },
                {
                    name: "Anh không theo đuổi em nữa...",
                    artist: "An Vũ Cover",
                    cover: "/img/8.jpg",
                    source: "/mp3/8.mp3",
                    url: "https://www.youtube.com/watch?v=LPntyq8cC1M&ab_channel=Hyarways",
                    favorited: false
                },
                {
                    name: "Anh biết-Xám",
                    artist: "D.Blue",
                    cover: "/img/9.jpg",
                    source: "/mp3/9.mp3",
                    url: "https://www.youtube.com/watch?v=MvopCo79pK4&ab_channel=MochiRoln-chill",
                    favorited: false
                }
            ],
            currentTrack: null,
            currentTrackIndex: 0,
            transitionName: null
        };
    },
    methods: {
        play() {
            if (this.audio.paused) {
                this.audio.play();
                this.isTimerPlaying = true;
            } else {
                this.audio.pause();
                this.isTimerPlaying = false;
            }
        },
        generateTime() {
            let width = (100 / this.audio.duration) * this.audio.currentTime;
            this.barWidth = width + "%";
            this.circleLeft = width + "%";
            let durmin = Math.floor(this.audio.duration / 60);
            let dursec = Math.floor(this.audio.duration - durmin * 60);
            let curmin = Math.floor(this.audio.currentTime / 60);
            let cursec = Math.floor(this.audio.currentTime - curmin * 60);
            if (durmin < 10) {
                durmin = "0" + durmin;
            }
            if (dursec < 10) {
                dursec = "0" + dursec;
            }
            if (curmin < 10) {
                curmin = "0" + curmin;
            }
            if (cursec < 10) {
                cursec = "0" + cursec;
            }
            this.duration = durmin + ":" + dursec;
            this.currentTime = curmin + ":" + cursec;
        },
        updateBar(x) {
            let progress = this.$refs.progress;
            let maxduration = this.audio.duration;
            let position = x - progress.offsetLeft;
            let percentage = (100 * position) / progress.offsetWidth;
            if (percentage > 100) {
                percentage = 100;
            }
            if (percentage < 0) {
                percentage = 0;
            }
            this.barWidth = percentage + "%";
            this.circleLeft = percentage + "%";
            this.audio.currentTime = (maxduration * percentage) / 100;
            this.audio.play();
        },
        clickProgress(e) {
            this.isTimerPlaying = true;
            this.audio.pause();
            this.updateBar(e.pageX);
        },
        prevTrack() {
            this.transitionName = "scale-in";
            this.isShowCover = false;
            if (this.currentTrackIndex > 0) {
                this.currentTrackIndex--;
            } else {
                this.currentTrackIndex = this.tracks.length - 1;
            }
            this.currentTrack = this.tracks[this.currentTrackIndex];
            this.resetPlayer();
        },
        nextTrack() {
            this.transitionName = "scale-out";
            this.isShowCover = false;
            if (this.currentTrackIndex < this.tracks.length - 1) {
                this.currentTrackIndex++;
            } else {
                this.currentTrackIndex = 0;
            }
            this.currentTrack = this.tracks[this.currentTrackIndex];
            this.resetPlayer();
        },
        resetPlayer() {
            this.barWidth = 0;
            this.circleLeft = 0;
            this.audio.currentTime = 0;
            this.audio.src = this.currentTrack.source;
            setTimeout(() => {
                if (this.isTimerPlaying) {
                    this.audio.play();
                } else {
                    this.audio.pause();
                }
            }, 300);
        },
        favorite() {
            this.tracks[this.currentTrackIndex].favorited = !this.tracks[
                this.currentTrackIndex
            ].favorited;
        }
    },
    created() {
        let vm = this;
        this.currentTrack = this.tracks[0];
        this.audio = new Audio();
        this.audio.src = this.currentTrack.source;
        this.audio.ontimeupdate = function () {
            vm.generateTime();
        };
        this.audio.onloadedmetadata = function () {
            vm.generateTime();
        };
        this.audio.onended = function () {
            vm.nextTrack();
            this.isTimerPlaying = true;
        };

        // this is optional (for preload covers)
        for (let index = 0; index < this.tracks.length; index++) {
            const element = this.tracks[index];
            let link = document.createElement('link');
            link.rel = "prefetch";
            link.href = element.cover;
            link.as = "image"
            document.head.appendChild(link)
        }
    }
});
