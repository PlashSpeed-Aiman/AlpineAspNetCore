function init() {
    Alpine.data('myComponent', () => ({
        zone: "",
        selectedZone: "",
        open: true,
        prayerTimes: [],
        toggle() {
            this.open = !this.open
        },
        getZoneData() {
            fetch(`https://localhost:7180/api/Solat/${this.selectedZone}`).then(
                response => response.json()
            ).then(data => {
                this.prayerTimes = []
                data.map(x => {
                    this.prayerTimes.push(x)
                })
                console.debug(data)
            })
        },
    }));
}